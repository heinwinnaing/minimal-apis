using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalApis.Services;

public class JwtTokenService
{
    public string CreateAccessToken(Claim[] jwtClaims, out DateTime expiryIn)
    {
        expiryIn = DateTime.UtcNow.AddMinutes(60);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8x95L5fXbqSgJhwK2nobqF7lUa5MQjEmnswG"));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            expires: expiryIn,
            claims: jwtClaims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
