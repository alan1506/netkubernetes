using System.Net;

namespace NetKubernetes.Middelware;

public class MiddelwareException : Exception
{
    public HttpStatusCode Codigo { get; set; }

    public object? Errores { get; set; }

    public MiddelwareException(HttpStatusCode codigo, object? errores)
    {
        Codigo = codigo;
        Errores = errores;
    }
}