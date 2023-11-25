using System.Net;
using NetKubernetes.Dtos.UsuarioDtos;
using Microsoft.AspNetCore.Identity;
using NetKubernetes.Models;
using NetKubernetes.Token;
using NetKubernetes.Middelware;
using Microsoft.EntityFrameworkCore;

namespace NetKubernetes.Data.Usuarios;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly UserManager<UsuarioModel> _userManager;
    private readonly SignInManager<UsuarioModel> _signInManager;
    private readonly IJwtGenerador _jwtGenerator;
    private readonly AppDbContext _contexto;
    private readonly IUsuarioSesion _usuarioSesion;

    public UsuarioRepository(UserManager<UsuarioModel> userManager,SignInManager<UsuarioModel> signInManager,
    IJwtGenerador jwtGenerator, AppDbContext contexto, IUsuarioSesion usuarioSesion)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _contexto = contexto;
        _usuarioSesion = usuarioSesion;
    }

    private UsuarioResponseDto TransformerUserToUserDto(UsuarioModel usuario)
    {
        return new UsuarioResponseDto{
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Telefono = usuario.Telefono,
            Email = usuario.Email,
            UserName = usuario.UserName,
            Token = _jwtGenerator.CrearToken(usuario)
        };
    }
    public async Task<UsuarioResponseDto> GetUsuario()
    {
        var usuario = await _userManager.FindByNameAsync(_usuarioSesion.ObtenerUsuarioSesion());
        if(usuario is null)
        {
            throw new MiddelwareException(HttpStatusCode.Unauthorized, 
            new {mensaje = "El usuario del token no existe ne la base de datos"});
        }
        return TransformerUserToUserDto(usuario!);
    }

    public async Task<UsuarioResponseDto> Login(UsuarioLoginRequestDto request)
    {
        var usuario = await _userManager.FindByEmailAsync(request.Email!);

        if(usuario is null)
        {
            throw new MiddelwareException(HttpStatusCode.Unauthorized, 
            new {mensaje = "El email del usuario no existe en mi base de datos"});
        }

       var resultado =  await _signInManager.CheckPasswordSignInAsync(usuario!, request.Password!, false);

       if(resultado.Succeeded)
       {
            return TransformerUserToUserDto(usuario!);
       }

        throw new MiddelwareException(
            HttpStatusCode.Unauthorized,
            new { mensaje = "Las credenciales son incorrectas" }
        );
    }

    public async Task<UsuarioResponseDto> RegistroUsuario(UsuarioRegistroRequestDto request)
    {
        var existeEmail = await _contexto.Users.Where( x => x.Email == request.Email).AnyAsync();
        if(existeEmail){
            throw new MiddelwareException(
            HttpStatusCode.BadRequest,
            new { mensaje = "El email del usuario ya existe en la base de datos." }
        );
        }

        var existeUserName = await _contexto.Users.Where( x => x.UserName == request.UserName).AnyAsync();
        if(existeUserName){
            throw new MiddelwareException(
            HttpStatusCode.BadRequest,
            new { mensaje = "El user name del usuario ya existe en la base de datos." }
        );
        }

        var usuario = new UsuarioModel {
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Telefono = request.Telefono,
            Email = request.Email,
            UserName = request.UserName
        };

       var resultado = await _userManager.CreateAsync(usuario!, request.Password!);

       if(resultado.Succeeded)
       {
            return TransformerUserToUserDto(usuario);
       }
        throw new Exception("No se pudo registrar el usuario");
        
    }
}