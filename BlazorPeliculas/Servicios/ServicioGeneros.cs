
using BlazorPeliculas.Datos;
using BlazorPeliculas.DTOs;
using BlazorPeliculas.Entidades;
using BlazorPeliculas.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Servicios;

public class ServicioGeneros(IDbContextFactory<ApplicationDbContext> dbFactory) : IServicioGeneros
{
    public async Task Actualizar(Genero genero)
    {
        using var context = dbFactory.CreateDbContext();
        context.Update(genero);
        await context.SaveChangesAsync();
    }

    public async Task<bool> Borrar(int id)
    {
        using var context = dbFactory.CreateDbContext();
        // en la tabla de generos busca aquellos generos cuyo Id sea igual al id que se le paso por parámetro
        // y luego ejecuta la eliminación de forma asíncrona utilizando el método "ExecuteDeleteAsync".
        // El resultado de esta operación se almacena en la variable "elementosBorrados",
        // que indica la cantidad de registros eliminados. Si el valor de "elementosBorrados" es mayor a cero,
        // significa que se eliminó al menos un registro, por lo que se devuelve "true". En caso contrario,
        // si no se eliminó ningún registro, se devuelve "false".
        var elementosBorrados = await context.Generos.Where(g => g.Id == id).ExecuteDeleteAsync();
        return elementosBorrados == 1;

    }

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

    public async Task<Genero?> ObtenerPorId(int id)
    {
        using var context = dbFactory.CreateDbContext();
        return await context.Generos.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Genero>> ObtenerTodos()
    {
        using var context = dbFactory.CreateDbContext();
        return await context.Generos.OrderBy(x => x.Nombre).ToListAsync();
    }
}
