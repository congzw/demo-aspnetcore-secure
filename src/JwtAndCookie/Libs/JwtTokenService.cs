using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAndCookie.Boots;
using Microsoft.IdentityModel.Tokens;

namespace JwtAndCookie.Libs
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
            
            //todo: remove this line!
            //为了测试方便，自动补齐Token
            //return token to http request client, and set header by client!
            MockClientRequest.Instance.Token = jwtToken;

            return jwtToken;
        }
    }
}
