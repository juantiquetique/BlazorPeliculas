using BlazorPeliculas.DTOs;
using BlazorPeliculas.Entidades;

namespace BlazorPeliculas.Servicios;

public interface IServicioPeliculas
{   
    Task<int> Crear(Pelicula pelicula);
    Task<HomeDTO> ObtenerPeliculasHome();
}
