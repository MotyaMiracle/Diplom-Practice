using Yard_Management_System;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
using (ApplicationContext bd = new ApplicationContext())
app.MapGet("/", () => "Hello World!");

app.Run();
