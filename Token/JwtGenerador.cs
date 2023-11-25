using System.Text;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NetKubernetes.Models;
using Microsoft.IdentityModel.Tokens;

namespace NetKubernetes.Token;

public class JwtGenerador : IJwtGenerador
{
    public string CrearToken(UsuarioModel usuarioModel)
    {
        var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.NameId, usuarioModel.UserName!),
            new Claim("userId", usuarioModel.Id),
            new Claim("email", usuarioModel.Email!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescripcion = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires =  DateTime.Now.AddDays(30),
            SigningCredentials = credenciales
        };

        var tokenHandler =  new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescripcion);
        return tokenHandler.WriteToken(token);
    }
}