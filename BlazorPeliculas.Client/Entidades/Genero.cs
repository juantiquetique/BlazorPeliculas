using BlazorPeliculas.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace BlazorPeliculas.Client.Entidades;

public class Genero
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo {0} es requerido.")]
    [PrimeraLetraMayuscula]
    public string? Nombre { get; set; }

    //relacion muchos a muchos entre genero y pelicula, un genero puede tener muchas peliculas y una pelicula puede tener muchos generos
    //A esto se le llama propiedad de navegacion, es decir, una propiedad que me permite navegar a la entidad relacionada, en este caso, genero pelicula
    public List<GeneroPelicula> GenerosPelicula { get; set; } = [];
}
