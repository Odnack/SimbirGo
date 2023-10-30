using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Simbir.GO.Entities.DbEntities;
using Simbir.GO.Entities.Options;

namespace Simbir.Go.DataAccess.Helpers;

public class TokenHelper
{
    private readonly JwtOptions _jwtOptions;
    
    public TokenHelper(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(User user)
    {
        var tokenKey = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var jwt = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(10),
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256),
            claims: claims
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
