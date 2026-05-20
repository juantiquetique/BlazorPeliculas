using BlazorPeliculas.Client.DTOs;
using BlazorPeliculas.Client.Entidades;
using MudBlazor;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorPeliculas.Client.Servicios
{
    public class ServicioPeliculasHttp(HttpClient httpClient) : IServicioPeliculas
    {
        //Esto configura cómo funciona el serializador JSON. ¿Qué es serializar? Convertir objeto C# → JSON
        //Deserializar: Convertir JSON → objeto C#
        private JsonSerializerOptions jsonSerializerOptions = 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; //para que no importe si el JSON tiene mayusculas o minusculas en los nombres de las propiedades
        public Task Actualizar(Pelicula pelicula)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Borrar(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoPaginadoDTO<Pelicula>> Buscar(ParametrosBusquedaPeliculaDTO parametros)
        {
            throw new NotImplementedException();
        }

        public Task<int> Crear(Pelicula pelicula)
        {
            throw new NotImplementedException();
        }

        //devuelve una pelicula o null si no existe (?)
        public async Task<PeliculaDetalleDTO?> ObtenerDetalle(int id)
        {
            var respuesta = await httpClient.GetAsync($"api/peliculas/{id}");
            if(respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            var cuerpo = await respuesta.Content.ReadAsStringAsync();
            var modelo = JsonSerializer.Deserialize<PeliculaDetalleDTO>(cuerpo, jsonSerializerOptions);//aqui se convierte el json a un objeto C# de tipo PeliculaDetalleDTO
            return modelo;
        }

        public Task<EditarPeliculaDTO?> ObtenerEditarPelicula(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HomeDTO> ObtenerPeliculasHome()
        {
            throw new NotImplementedException();
        }
    }
}
