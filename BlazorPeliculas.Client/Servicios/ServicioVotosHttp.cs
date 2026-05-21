using BlazorPeliculas.Client.DTOs;
using System.Net.Http.Json;

namespace BlazorPeliculas.Client.Servicios
{
    public class ServicioVotosHttp(HttpClient httpClient) : IServicioVotos
    {
        public async Task Votar(VotoPeliculaDTo votoPeliculaDTo)
        {
            await httpClient.PostAsJsonAsync("api/votos", votoPeliculaDTo);
        }
    }
}
