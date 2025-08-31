using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api;

public class TokenServico : ITokenServico
{
    private readonly IConfiguration _configuration;


    public TokenServico(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public string GerarToken(Administrador administrador)
    {
        var jwtKey = this._configuration["jwt"];

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new Exception("A chave jwt não foi configurada corretamente no arquivo appsetings.json");
        }

        var senhaSegura = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credencial = new SigningCredentials(senhaSegura, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim("Email", administrador.Email),
            new Claim("Senha", administrador.Senha),
            new Claim(ClaimTypes.Role, administrador.Perfil)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credencial
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
