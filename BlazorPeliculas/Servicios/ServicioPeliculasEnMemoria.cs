using BlazorPeliculas.Entidades;

namespace BlazorPeliculas.Servicios;

public class ServicioPeliculasEnMemoria : IServicioPeliculas
{
    public List<Pelicula> ObternerPeliculas()
    {
        return new List<Pelicula>
        {
             new Pelicula
             {
                 Id = 1,
                 Titulo = "Captain America: Brave New World",
                 FechaLanzamiento = new DateTime(2025,2,14)
             },
             new Pelicula
             {
                 Id = 2,
                 Titulo = "Mission: Impossible - Dead Reckoning Part two",
                 FechaLanzamiento = new DateTime(2025,5,23)
             }
        };
    }
}
