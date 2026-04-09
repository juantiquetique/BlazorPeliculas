
using BlazorPeliculas.Datos;
using BlazorPeliculas.Entidades;
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

    public async Task<IEnumerable<Genero>> Obtener()
    {
        using var context= dbFactory.CreateDbContext();
        return await context.Generos.OrderBy(g=>g.Nombre).AsNoTracking().ToListAsync();
    }
}
