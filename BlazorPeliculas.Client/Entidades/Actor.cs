using BlazorPeliculas.Client.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorPeliculas.Client.Entidades;

public class Actor
{
    public int Id { get; set; }
    [Required(ErrorMessage ="El campo {0} es requerido")]
    public string? Nombre { get; set; }
    public string? FotoURL { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public List<ActorPelicula> ActoresPeliculas { get; set; } = [];

    //el NotMapped es para indicar que esta propiedad no se va a mapear a la base de datos,
    //es decir, que no va a ser una columna de la tabla, esto es útil para propiedades que solo se utilizan en la lógica de la aplicación y no necesitan ser almacenadas en la base de datos
    [NotMapped]
    public IBrowserFile? FotoArchivo { get; set; }
    [NotMapped]
    public string? Personaje { get; set; }
    [NotMapped]
    public ArchivoDTO? Archivo { get; set; }

    // se sobreescribe el método equals para comparar dos actores por su id,
    // esto es útil para evitar problemas de referencia al comparar objetos en la lógica de la aplicación,
    // por ejemplo, al eliminar un actor de una película, se puede comparar el id del actor con el id del actor a
    // eliminar en lugar de comparar las referencias de los objetos, lo que puede causar problemas si los objetos
    // no son exactamente los mismos en memoria
    public override bool Equals(object? obj)
    {
        if(obj is Actor a2)
        {
            return Id == a2.Id;
        }
        return false;
    }
    // se sobreescribe el método GetHashCode para que sea consistente con el método Equals, es decir,
    // que dos objetos que son iguales según el método Equals tengan el mismo código hash,
    // esto es importante para el correcto funcionamiento de las colecciones que utilizan códigos hash,
    // como los diccionarios o los conjuntos
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
