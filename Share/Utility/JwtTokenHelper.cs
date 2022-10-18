using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Share.Utility;

public class JwtTokenHelper
{
  public string GenerateToken(string userId, string role = "user")
  {
    var config = CustomConfigurationHelper.CreateConfigurationBuilder("./CommonConfiguration/Configuration.json");
    config = CustomConfigurationHelper.CreateConfigurationBuilder("./Configuration/Configuration.json");

    var now = DateTime.UtcNow;
    dynamic token = config.GetSection("TokenManagement").Get<ExpandoObject>();
    int.TryParse(token.AccessExpiration, out int accessExpiration);
    var claims = new[]
        {
                    new Claim("Name", userId),
                    new Claim("Role", role),
                    new Claim("Ticks", now.AddMinutes(accessExpiration).Ticks.ToString())
                };



    var jwtToken = new JwtSecurityToken(
    token.Issuer,
    token.Audience,
    claims,
    expires: now.AddMinutes(accessExpiration),
    //signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)), SecurityAlgorithms.HmacSha512Signature));
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token.Secret)), SecurityAlgorithms.HmacSha512Signature));
    return new JwtSecurityTokenHandler().WriteToken(jwtToken);

  }
}