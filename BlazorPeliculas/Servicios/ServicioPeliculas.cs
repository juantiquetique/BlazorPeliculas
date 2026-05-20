using BlazorPeliculas.Datos;
using BlazorPeliculas.Client.DTOs;
using BlazorPeliculas.Client.Entidades;
using BlazorPeliculas.Utilidades;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BlazorPeliculas.Client.Servicios;

namespace BlazorPeliculas.Servicios
{
    //IHttpContextAccessor: para obtener el usuario principal
    public class ServicioPeliculas(IDbContextFactory<ApplicationDbContext> dbFactory,
        IAlmacenadorArchivos almacenadorArchivos, IHttpContextAccessor httpContextAccessor) : IServicioPeliculas
    {
        private readonly string contenedor = "peliculas";

        public async Task Actualizar(Pelicula pelicula)
        {
            if (pelicula.Archivo is not null)
            {
                pelicula.PosterURL = await almacenadorArchivos.Editar(pelicula.PosterURL,
                    contenedor, pelicula.Archivo);
            }
            using var context = dbFactory.CreateDbContext();
                        
            var peliculaDB = await context.Peliculas
                .Include(p => p.GenerosPelicula)
                .Include(p => p.ActoresPelicula)
                .FirstAsync(p => p.Id == pelicula.Id);

            context.Entry(peliculaDB).CurrentValues.SetValues(pelicula);
            var generosIds = pelicula.GenerosPelicula.Select(x => x.GeneroId).ToList();//obtengo los id de los generos
            SincronizarGeneros(peliculaDB, generosIds);//peliculaDB: version actual de la pelicula en la base de datos. generosIds los ids de los generos seleccionados
            SincronizarActores(peliculaDB, pelicula.ActoresPelicula);
            await context.SaveChangesAsync();
        }

        private void SincronizarGeneros(Pelicula pelicula, List<int>generosIds)
        {
            var actuales = pelicula.GenerosPelicula.Select(x => x.GeneroId); 
            pelicula.GenerosPelicula.RemoveAll(x => !generosIds.Contains(x.GeneroId)); //voy a remover aquellos que no se encuentren en el listado de generosid
            var faltantes = generosIds.Except(actuales); //cuales son los nuevos
            foreach(var generoId in faltantes)
            {
                pelicula.GenerosPelicula.Add(new GeneroPelicula
                {
                    PeliculaId = pelicula.Id,
                    GeneroId = generoId
                });
            }
        }

        private void SincronizarActores(Pelicula pelicula, 
            List<ActorPelicula> actoresSeleccionados)
        {
            var actuales = pelicula.ActoresPelicula.ToList();
            var actoresSeleccionadosIds = actoresSeleccionados.Select(x => x.ActorId).ToList();
            pelicula.ActoresPelicula.RemoveAll(x => !actoresSeleccionadosIds.Contains(x.ActorId));

            foreach(var actorPelicula in pelicula.ActoresPelicula)
            {
                var actorPeliculaSeleccionado =
                    actoresSeleccionados.Single(x => x.ActorId == actorPelicula.ActorId)!;

                actorPelicula.Personaje = actorPeliculaSeleccionado.Personaje;
                actorPelicula.Orden = actorPeliculaSeleccionado.Orden;
            }

            var actoresACtualesIds = pelicula.ActoresPelicula.Select(x => x.ActorId);
            var faltantesIds = actoresSeleccionadosIds.Except(actoresACtualesIds);

            foreach(var actorId in faltantesIds)
            {
                var actorPeliculaSeleccionado =
                    actoresSeleccionados.Single(x => x.ActorId == actorId)!;

                pelicula.ActoresPelicula.Add(new ActorPelicula
                {
                    PeliculaId = actorPeliculaSeleccionado.PeliculaId,
                    ActorId = actorPeliculaSeleccionado.ActorId,
                    Orden = actorPeliculaSeleccionado.Orden,
                    Personaje = actorPeliculaSeleccionado.Personaje
                });
            };
        }

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

            //se implemento votación
            if(parametros.MasVotadas)
            {
                ///ordena de forma descendente las peliculas según el promedio de votos
                peliculasQueryable = peliculasQueryable.OrderByDescending(p => p.VotosPelicula.Average(vp => vp.Voto));
            }

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

            // Sistema de Votacion
            var promedioVoto = 0.0;
            var votoUsuario = 0;

            //realiza la conexion a la base de datos va a la tabla VotosPeliculas para obtener el voto del usuario logueado
            //con la x crea un "alias" para la tabla VotosPeliculas, luego se filtra por el campo PeliculaId
            if (await context.VotosPeliculas.AnyAsync(x => x.PeliculaId == id)) 
            {
                //si existen votos para esa pelicula, se calcula el promedio de los votos utilizando el método AverageAsync,
                //que toma el campo Voto de la tabla VotosPeliculas.
                promedioVoto = await context.VotosPeliculas.Where(x => x.PeliculaId == id)
                    .AverageAsync(x => x.Voto);

                //si el usuario esta autenticado, se obtiene el id del usuario logueado y se busca en la tabla VotosPeliculas si existe un voto para esa película y ese usuario.
                if (httpContextAccessor.HttpContext is not null && httpContextAccessor.HttpContext.User.Identity!.IsAuthenticated)
                {
                    var usuarioId = httpContextAccessor.HttpContext
                        .User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                    //busca en la tabla VotosPeliculas un registro que coincida con el id de la película y el id del usuario, utilizando el método FirstOrDefaultAsync.
                    var votoUsuarioDB = await context.VotosPeliculas
                        .FirstOrDefaultAsync(x => x.PeliculaId == id && x.UsuarioId == usuarioId);

                    if (votoUsuarioDB is not null)
                    {
                        votoUsuario = votoUsuarioDB.Voto;
                    }
                }
            }

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

        public async Task<EditarPeliculaDTO?> ObtenerEditarPelicula(int id)
        {
            var peliculaDetalle = await ObtenerDetalle(id);
            if (peliculaDetalle is null) { return null; }

            using var context = dbFactory.CreateDbContext();
            var generosSeleccionadosIds = peliculaDetalle!.Generos.Select(x => x.Id).ToList();
            var generosNoSelecionados = await context.Generos
                                        .Where(x => !generosSeleccionadosIds.Contains(x.Id))
                                        .ToListAsync();

            var modelo = new EditarPeliculaDTO(peliculaDetalle.Pelicula,
                peliculaDetalle.Actores, peliculaDetalle.Generos, generosNoSelecionados);
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

        public async Task<bool> Borrar(int id)
        {
            using var context = dbFactory.CreateDbContext();
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(p => p.Id == id);
            if (pelicula is null)
            {
                return false;
            }

            context.Remove(pelicula);
            await context.SaveChangesAsync();
            await almacenadorArchivos.Borrar(pelicula.PosterURL,contenedor);
            return true;
        }
    }
}
