using BlazorPeliculas.Datos;
using BlazorPeliculas.DTOs;
using BlazorPeliculas.Entidades;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorPeliculas.Servicios;
//IHttpContextAccessor: se usa para acceder al contexto HTTP actual, lo que permite obtener información sobre el usuario autenticado y otros detalles de la solicitud.
//IDbContextFactory: acceder a la base de datos desde servicios o componentes que no son controladores o páginas Razor, como en este caso, un servicio de votación.
public class ServicioVotos(IHttpContextAccessor httpContextAccessor,
    IDbContextFactory<ApplicationDbContext> dbFactory) : IServicioVotos
{
    public async Task Votar(VotoPeliculaDTo votoPeliculaDTo)
    {
        //obtener el usuario autenticado actual que está haciendo la petición en una aplicación ASP.NET Core.
        var usuario = httpContextAccessor.HttpContext!.User; 

        //si el usuario es nulo o no esta autenticado, no se hace nada
        if (usuario.Identity is not null && !usuario.Identity.IsAuthenticated)
        {
            return;
        }

        //obtener el id del usuario logueado. (como tomar el s_id)
        var usuarioId = usuario.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        //crea la conexion a la base de datos utilizando el factory, lo que permite acceder a los datos de las votaciones de películas.(como hacer un USE datosVotacionesPeliculas SHARED)
        var context = dbFactory.CreateDbContext();

        //se busca en VotosPeliculas si ya existe un voto para la película y el usuario actual. (como hacer un SEEK() con la pelicula y el usuario ala tabla VotosPeliculas)
        var votoActual = await context.VotosPeliculas
            .FirstOrDefaultAsync(x => x.PeliculaId == votoPeliculaDTo.PeliculaId 
            && x.UsuarioId == usuarioId);

        //si no existe se crea un nuevo voto, se asigna la fecha actual, el id de la película, el voto y el id del usuario, y se agrega a la base de datos.
        //Si ya existe, se actualiza la fecha y el voto.
        if (votoActual == null)
        {
            var votoPelicula = new VotoPelicula
            {
                FechaVoto = DateTime.UtcNow,
                PeliculaId = votoPeliculaDTo.PeliculaId,
                Voto = votoPeliculaDTo.Voto,
                UsuarioId = usuarioId
            };
            context.Add(votoPelicula);
        }else
        {
            votoActual.FechaVoto = DateTime.UtcNow;
            votoActual.Voto = votoPeliculaDTo.Voto;
        }

        //se guardan los cambios en la base de datos, lo que actualiza o inserta el voto según corresponda.
        await context.SaveChangesAsync();
    }
}
