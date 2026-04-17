using BlazorPeliculas.DTOs;
using BlazorPeliculas.Entidades;

namespace BlazorPeliculas.Servicios;

public interface IServicioGeneros
{
    Task<int> Crear(Genero genero);
    //me va a permitir todos los generos que tengo en la base de datos
    //Task<IEnumerable<Genero>> Obtener();

    //con este metodo me va a permitir obtener los generos de forma paginada,
    //es decir, me va a devolver un resultado paginado con los generos y la cantidad total de registros
    Task<ResultadoPaginadoDTO<Genero>> Obtener(PaginacionDTO paginacionDTO);

    //este método me va a permitir actualizar un género existente en la base de datos,
    //recibe un objeto "Genero" con los datos actualizados y realiza la operación de actualización de forma asíncrona
    Task Actualizar(Genero genero);
    Task<Genero?> ObtenerPorId(int id);

    Task<bool> Borrar(int id);

    Task<IEnumerable<Genero>> ObtenerTodos();
}
