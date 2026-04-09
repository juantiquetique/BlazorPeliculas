using BlazorPeliculas.Entidades;

namespace BlazorPeliculas.Servicios;

public interface IServicioGeneros
{
    Task<int> Crear(Genero genero);
    //me va a permitir todos los generos que tengo en la base de datos
    Task<IEnumerable<Genero>> Obtener();
}
