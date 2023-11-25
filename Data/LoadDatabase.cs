using Microsoft.AspNetCore.Identity;
using NetKubernetes.Models;

namespace NetKubernetes.Data;
public class LoadDataBase
{
    public static async Task InsertData(AppDbContext context,
     UserManager<UsuarioModel> usuarioManager)
    {
        if(!usuarioManager.Users.Any())
        {
            var usuario = new UsuarioModel()
            {
                Nombre = "Alan",
                Apellido = "De los santos",
                Email = "ala@yo.com",
                UserName = "alandelos",
                Telefono = "234234243"
            };

            await usuarioManager.CreateAsync(usuario, "Alansd123...");
        }

        if(!context.InmuebleModel!.Any())
        {
            context.InmuebleModel!.AddRange(
                new InmuebleModel{
                    Nombre = "Casa de playa",
                    Direccion= "av el sol",
                    Precio = 4500M,
                    FechaCreacion = DateTime.Now
                },
                new InmuebleModel{
                    Nombre = "Casa de invierno",
                    Direccion= "av la roca",
                    Precio = 3500M,
                    FechaCreacion = DateTime.Now
                }
            );
        }

        context.SaveChanges();
    }
}