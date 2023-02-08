﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Yard_Management_System.Models;

namespace Yard_Management_System
{
    public class Authorization 
    {
        public static ClaimsIdentity GetIdentity(User? user)
        {
            if (user == null)
                return null;

            var role = user.Role.Name;
            if (role == null)
                return null;

            var passwordHash = GetHash(user.Password);
            if (passwordHash != user.PasswordHash)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
                new Claim("Id", user.Id.ToString())
            };

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
        public static string GenerateJwtToken(IEnumerable<Claim> claims, TimeSpan lifetime)
        {
            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                issuer: SigningOptions.SignIssuer,
                audience: SigningOptions.SignAudience,
                notBefore: now,
                claims: claims,
                expires: now + lifetime,
                signingCredentials: new SigningCredentials(SigningOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public static string GetHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashValue = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
                var passwordHash = Encoding.ASCII.GetString(hashValue);
                return passwordHash;
            }
           
        }
    }
}