
using BlazorPeliculas.Datos;
using BlazorPeliculas.DTOs;
using BlazorPeliculas.Entidades;
using BlazorPeliculas.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Servicios;

public class ServicioGeneros(IDbContextFactory<ApplicationDbContext> dbFactory) : IServicioGeneros
{
    //como nos vamos a comunicar con la base de datos la buena práctica es utilizar programación asíncrona
    public async Task<int> Crear(Genero genero)
    {
        // se instancia el dbcontex por un momento y luego se termina utilizando el factory, se agrega el nuevo género y se guardan los cambios
        using var context = dbFactory.CreateDbContext();
        context.Add(genero);
        await context.SaveChangesAsync();
        return genero.Id;
    }

    public async Task<ResultadoPaginadoDTO<Genero>> Obtener(PaginacionDTO paginacionDTO)
    {
        using var context= dbFactory.CreateDbContext();
        var elementos = await context.Generos.OrderBy(g => g.Nombre)
            .Paginar(paginacionDTO)
            .AsNoTracking().ToListAsync();// se obtiene la lista de géneros ordenados por nombre, se aplica la paginación utilizando el método de extensión "Paginar" y se convierte a una lista de forma asíncrona. El método "AsNoTracking" se utiliza para mejorar el rendimiento al indicar que no se realizarán cambios en los objetos recuperados.
        var conteo = await context.Generos.CountAsync();// se obtiene el conteo total de registros para poder mostrarlo en la interfaz de usuario y así saber cuántas páginas hay en total
        var respuesta = new ResultadoPaginadoDTO<Genero>
        {
            CantidadTotalRegistros = conteo,
            Elemento = elementos
        };

        return respuesta;
    }
}
