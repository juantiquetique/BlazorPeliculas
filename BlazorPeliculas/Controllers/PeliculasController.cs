using BlazorPeliculas.Client.DTOs;
using BlazorPeliculas.Client.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace BlazorPeliculas.Controllers
{
    //“Estoy desarrollando un endpoint REST en ASP.NET Core usando arquitectura por capas, inyección de dependencias y DTOs.”
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController(IServicioPeliculas servicioPeliculas) : ControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeliculaDetalleDTO>> ObtenerPorId(int id)
        {
            var pelicula = await servicioPeliculas.ObtenerDetalle(id);
            if (pelicula is null)
            {
                return NotFound();
            }
            return pelicula;
        }
    }
}
