using BlazorPeliculas.Client.DTOs;
using BlazorPeliculas.Client.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace BlazorPeliculas.Controllers
{
    [Route("api/votos")]
    [ApiController]
    public class VotosController(IServicioVotos servicioVotos): ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Post(VotoPeliculaDTo votoPeliculaDTo)
        {
            await servicioVotos.Votar(votoPeliculaDTo);
            return Ok();
        }
    }
}
