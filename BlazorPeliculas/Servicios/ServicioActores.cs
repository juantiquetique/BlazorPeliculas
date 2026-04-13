using BlazorPeliculas.Datos;
using BlazorPeliculas.DTOs;
using BlazorPeliculas.Entidades;
using BlazorPeliculas.Utilidades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Servicios
{
    public class ServicioActores(IDbContextFactory<ApplicationDbContext> dbFactory,
        IAlmacenadorArchivos almacenadorArchivos) : IServicioActores
    {
        private readonly string contenedor = "actores";

        public async Task<bool> Borrar(int id)
        {
            using var context = dbFactory.CreateDbContext();
            var actor = await context.Actor.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
            {
                return false;
            }

            context.Remove(actor);
            await context.SaveChangesAsync();
            await almacenadorArchivos.Borrar(actor.FotoURL, contenedor);

            return true;
        }

        public async Task<int> Crear(Actor actor)
        {
            if(actor.Archivo is not null)
            {
                actor.FotoURL = await almacenadorArchivos.Almacenar(contenedor, actor.Archivo);
            }

            using var context = dbFactory.CreateDbContext();
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<ResultadoPaginadoDTO<Actor>> Obtener(PaginacionDTO paginacionDTO)
        {
            using var context = dbFactory.CreateDbContext();
            var elementos = await context.Actor.OrderBy(x => x.Nombre)
                .Paginar(paginacionDTO)
                .AsNoTracking().ToListAsync();

            var conteo = await context.Actor.CountAsync();

            var respuesta = new ResultadoPaginadoDTO<Actor>
            {
                CantidadTotalRegistros = conteo,
                Elemento = elementos
            };

            return respuesta;
        }
    }
}
