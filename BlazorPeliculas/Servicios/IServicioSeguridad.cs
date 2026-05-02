using BlazorPeliculas.DTOs;

namespace BlazorPeliculas.Servicios
{
    public interface IServicioSeguridad
    {
        Task<ResultadoPaginadoDTO<UsuarioDTO>> Obtener(PaginacionDTO paginacionDTO);
        Task<bool> HacerAdmin(string email);
        Task<bool> RemoverAdmin(string email);
    }
}
