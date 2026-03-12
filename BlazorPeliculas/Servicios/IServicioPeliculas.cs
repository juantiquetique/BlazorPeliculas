using BlazorPeliculas.Entidades;

namespace BlazorPeliculas.Servicios;

public interface IServicioPeliculas
{
    List<Pelicula> ObternerPeliculas();
}
