using NetKubernetes.Models;

namespace NetKubernetes.Data.Inmuebles;
public interface IInmuebleRepository{
    Task<bool> SaveChanges();

    Task<IEnumerable<InmuebleModel>> GetAllInmuebles();

    Task<InmuebleModel> GetInmuebleById(int id);

    Task CreateInmuble(InmuebleModel inmuebleModel);

    Task DeleteInmueble(int id);
}