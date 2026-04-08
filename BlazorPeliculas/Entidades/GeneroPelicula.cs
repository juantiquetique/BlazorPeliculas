namespace BlazorPeliculas.Entidades;

public class GeneroPelicula
{
    public string PeliculaId { get; set; }
    public int GeneroId { get; set; }
    public Genero? Genero { get; set; }
    public Pelicula? Pelicula { get; set; }
    }
