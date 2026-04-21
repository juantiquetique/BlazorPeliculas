using BlazorPeliculas.Datos;
using BlazorPeliculas.DTOs;
using BlazorPeliculas.Entidades;
using BlazorPeliculas.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Servicios
{
    public class ServicioPeliculas(IDbContextFactory<ApplicationDbContext> dbFactory,
        IAlmacenadorArchivos almacenadorArchivos) : IServicioPeliculas
    {
        private readonly string contenedor = "peliculas";

        public async Task<ResultadoPaginadoDTO<Pelicula>> Buscar(ParametrosBusquedaPeliculaDTO parametros)
        {
            using var context = dbFactory.CreateDbContext();
            var peliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(parametros.Titulo))
            {
                peliculasQueryable = peliculasQueryable
                                    .Where(x => x.Titulo!.Contains(parametros.Titulo));
            }

            if(parametros.EnCartelera)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCartelera);
            }

            if(parametros.Estrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaLanzamiento >= hoy);
            }

            if(parametros.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(p =>
                    p.GenerosPelicula.Select(gp => gp.GeneroId).Contains(parametros.GeneroId));
            }

            //TODO: implementar votación

            var peliculas = await peliculasQueryable.Paginar(parametros.PaginacionDTO).ToListAsync();
            var conteo = await peliculasQueryable.CountAsync();

            var respuesta = new ResultadoPaginadoDTO<Pelicula>
            {
                Elemento = peliculas,
                CantidadTotalRegistros = conteo
            };

            return respuesta;
        }

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

        public async Task<PeliculaDetalleDTO?> ObtenerDetalle(int id)
        {
            using var context = dbFactory.CreateDbContext();
            var pelicula = await context.Peliculas.Where(p => p.Id == id)
                .Include(p => p.GenerosPelicula)
                    .ThenInclude(gp => gp.Genero)
                .Include(p => p.ActoresPelicula.OrderBy(pa => pa.Orden))
                    .ThenInclude(pa => pa.Actor)
                .FirstOrDefaultAsync();

            if (pelicula is null)
            {
                return null;
            }

            //TODO: Sistema de Votacion
            var promedioVoto = 4;
            var votoUsuario = 5;

            var modelo = new PeliculaDetalleDTO();
            modelo.Pelicula = pelicula;
            modelo.Generos = pelicula.GenerosPelicula.Select(gp => gp.Genero!).ToList();
            modelo.Actores = pelicula.ActoresPelicula.Select(pa => new Actor
            {
                Id = pa.ActorId,
                Nombre = pa.Actor!.Nombre,
                Personaje = pa.Personaje,
                FotoURL = pa.Actor.FotoURL
            }).ToList();

            modelo.PromedioVotos = promedioVoto;
            modelo.VotoUsuario = votoUsuario;

            return modelo;
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
