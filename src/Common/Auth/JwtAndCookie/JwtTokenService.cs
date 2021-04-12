using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Common.Auth.JwtAndCookie
{
    public interface IJwtTokenService
    {
        string GenerateJsonWebToken(IEnumerable<Claim> claims);
    }

    public class JwtTokenService : IJwtTokenService
    {
        public string GenerateJsonWebToken(IEnumerable<Claim> claims)
        {
            //todo: inject
            var jwtSetting = JwtSetting.Instance;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(jwtSetting.Issuer,
                jwtSetting.Issuer,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var jwtToken
                = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }

    public class JwtSetting
    {
        public string Key { get; set; }
        public string Issuer { get; set; }

        //todo: read from config
        public static JwtSetting Instance = new JwtSetting()
        {
            Issuer = "TheIssuer",
            Key = "TheKey1234567890"
        };
    }
}
