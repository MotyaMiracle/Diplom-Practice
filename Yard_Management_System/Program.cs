using Yard_Management_System;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Serilog;
using Microsoft.AspNetCore.Authentication;
using System;
using Yard_Management_System.Models;

var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = SigningOptions.SignIssuer,
            ValidAudience = SigningOptions.SignAudience,
            IssuerSigningKey = SigningOptions.GetSymmetricSecurityKey(),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OnlyForAdmin", policy =>
    {
        policy.RequireClaim(ClaimsIdentity.DefaultRoleClaimType, "Admin");
    });
});
var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => Results.Redirect("/login"));


app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    // html-форма дл€ ввода логина/парол€
    string loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>METANIT.COM</title>
    </head>
    <body>
        <h2>Login Form</h2>
        <form method='post'>
            <p>
                <label>Login</label><br />
                <input name='login' />
            </p>
            <p>
                <label>Password</label><br />
                <input type='password' name='password' />
            </p>
            <input type='submit' value='Login' />
        </form>
    </body>
    </html>";
    await context.Response.WriteAsync(loginForm);
});


app.MapPost("/login", async (string? returnUrl, HttpContext context, ApplicationContext db) =>
{
    var form = context.Request.Form;

    if (!form.ContainsKey("login") || !form.ContainsKey("password"))
        return Results.BadRequest("Login и/или пароль не установлены");

    string login = form["login"];
    string password = form["password"];

    var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(p => p.Login == login && p.Password == password);
    if (user is null) return Results.Unauthorized();
    //  остыль чтобы задать хешированный пароль, а то при HasData не получалось
    if (user.PasswordHash == null)
    {
        user.PasswordHash = Authorization.GetHash(user.Password);
        await db.SaveChangesAsync();
    }
    var identity = Authorization.GetIdentity(user);
    var claims = identity.Claims.ToList();
    var encodedJwt = Authorization.GenerateJwtToken(claims, TimeSpan.FromDays(7));
    var claimsPrincipal = new ClaimsPrincipal(identity);
    var response = new
    {
        userRole = user.Role?.Name,
        Token = encodedJwt
    };
    return Results.Json(response);
});


app.MapGet("/admin/users", [Authorize(Policy = "OnlyForAdmin")] async (ApplicationContext db, HttpContext context) =>
{
    var users = await db.Users.Include(u => u.Role).ToListAsync();
    string usersToString = "";
    foreach(var u in users)
    {
        usersToString = $"{usersToString}\nLogin: {u.Login}, Email: {u.Email}, Role: {u.Role?.Name}, HashPassword: {u.PasswordHash}";
    }
    return Results.Content(usersToString);
});

app.Run();
