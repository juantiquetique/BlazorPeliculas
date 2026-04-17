using BlazorPeliculas.Datos;
using BlazorPeliculas.DTOs;
using BlazorPeliculas.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Servicios
{
    public class ServicioPeliculas(IDbContextFactory<ApplicationDbContext> dbFactory,
        IAlmacenadorArchivos almacenadorArchivos) : IServicioPeliculas
    {
        private readonly string contenedor = "peliculas";
        public async Task<int> Crear(Pelicula pelicula)
        {
            if (pelicula.Archivo is not null)
            {
                pelicula.PosterURL = await almacenadorArchivos.Almacenar(contenedor, pelicula.Archivo);
            }

            using var context = dbFactory.CreateDbContext();
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        public async Task<HomeDTO> ObtenerPeliculasHome()
        {
            var limite = 5;
            using var context = dbFactory.CreateDbContext();

            var catelera = await context.Peliculas
                .Where(x => x.EnCartelera)
                .OrderBy(x => x.FechaLanzamiento)
                .Take(limite).ToListAsync();

            var hoy = DateTime.Today;
            var futurosEstrenos = await context.Peliculas
                .Where(x => x.FechaLanzamiento > hoy)
                .OrderBy(x => x.FechaLanzamiento)
                .Take(limite).ToListAsync();

            var respuesta = new HomeDTO
            {
                EnCartelera = catelera,
                FuturosEstrenos = futurosEstrenos
            };

            return respuesta;
        }     
    }
}
