using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Common;

public class JwtHelper
{
    private readonly IConfiguration _configuration;

    public JwtHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(UserModel userModel)
    {
        // 1. 定义需要使用到的Claims
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub,userModel.UserId),
            new Claim(JwtRegisteredClaimNames.Iss, "wingwell"),
            new Claim(ClaimTypes.Name, userModel.UserName), //HttpContext.User.Identity.Name
            new Claim("UnionId", userModel.UnionId),
            new Claim("OpenId", userModel.OpenId)
        };
        foreach (var userRole in userModel.UserRoles)
        {
            //HttpContext.User.IsInRole("r_admin")
             claims.Add(new Claim(ClaimTypes.Role, userRole.RoleName));
        }
        // 2. 从 appsettings.json 中读取SecretKey
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

        // 3. 选择加密算法
        var algorithm = SecurityAlgorithms.HmacSha256;

        // 4. 生成Credentials
        var signingCredentials = new SigningCredentials(secretKey, algorithm);

        // 5. 根据以上，生成token
        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],     //Issuer
            _configuration["Jwt:Audience"],   //Audience
            claims.ToArray(),                          //Claims,
            DateTime.Now,                    //notBefore
            DateTime.Now.AddDays(1),    //expires
            signingCredentials               //Credentials
        );

        // 6. 将token变为string
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return token;
    }
    
}