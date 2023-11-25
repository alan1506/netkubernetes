using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetKubernetes.Middelware;
using NetKubernetes.Models;
using NetKubernetes.Token;

namespace NetKubernetes.Data.Inmuebles;

public class InmuebleRepository : IInmuebleRepository
{
    private readonly AppDbContext _contexto;
    private readonly IUsuarioSesion _usuarioSesion;
    private readonly UserManager<UsuarioModel> _userManager;

    public InmuebleRepository(AppDbContext contexto, IUsuarioSesion sesion, 
                                UserManager<UsuarioModel> userManager)
    {
        _contexto = contexto;
        _usuarioSesion = sesion;
        _userManager = userManager;
    }
    public async Task CreateInmuble(InmuebleModel inmuebleModel)
    {
        var usuario = await _userManager.FindByIdAsync(_usuarioSesion.ObtenerUsuarioSesion());

        if(usuario is null){
            throw new MiddelwareException(
                HttpStatusCode.Unauthorized,
                new { mensaje = "El usuario no es valido para hacer esta insercion"}
            );
        }

        if(inmuebleModel is null){
            throw new MiddelwareException(
                HttpStatusCode.Unauthorized,
                new { mensaje = "Los datos de inmueble son incorrectos"}
            );
        }

        inmuebleModel.FechaCreacion = DateTime.Now;
        inmuebleModel.UsuarioId = Guid.Parse(usuario!.Id);

        await _contexto.InmuebleModel!.AddAsync(inmuebleModel);
    }

    public async Task DeleteInmueble(int id)
    {
         var inmueble = await _contexto.InmuebleModel!.FirstOrDefaultAsync(x => x.Id == id);
         _contexto.InmuebleModel!.Remove(inmueble!);
    }

    public  async Task<IEnumerable<InmuebleModel>> GetAllInmuebles()
    {
        return await _contexto.InmuebleModel!.ToListAsync();
    }

    public async Task<InmuebleModel> GetInmuebleById(int id)
    {
        return await _contexto.InmuebleModel!.FirstOrDefaultAsync(x => x.Id == id)!;
    }

    public async Task<bool> SaveChanges()
    {
        return ((await _contexto.SaveChangesAsync()) >= 0);
    }
}