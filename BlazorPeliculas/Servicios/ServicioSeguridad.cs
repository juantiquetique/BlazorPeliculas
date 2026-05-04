using BlazorPeliculas.Datos;
using BlazorPeliculas.DTOs;
using BlazorPeliculas.Entidades;
using BlazorPeliculas.Utilidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Servicios;
//el usermanager es el que nos permite realizar operaciones sobre los usuarios en este caso vamos agregar y remover el rol administrador aun usuario, el dbcontextfactory es para crear instancias del contexto de la base de datos.
public class ServicioSeguridad(IDbContextFactory<ApplicationDbContext> dbFactory,
    UserManager<ApplicationUser>userManager) : IServicioSeguridad
{
    public async Task<bool> HacerAdmin(string email)
    {
        using var context = dbFactory.CreateDbContext();
        var usuario = await userManager.FindByEmailAsync(email);

        if(usuario is null)
        {
            return false;
        }
        await userManager.AddToRoleAsync(usuario, "administrador");
        await userManager.UpdateSecurityStampAsync(usuario);//se actualiza el security stamp para invalidar las cookies de autenticacion del usuario y forzar a que se vuelva a autenticar con los nuevos roles asignados.
        return true;
    }

    public async Task<ResultadoPaginadoDTO<UsuarioDTO>> Obtener(PaginacionDTO paginacionDTO)
    {
        using var context = dbFactory.CreateDbContext();
        var elementos = await context.Users.OrderBy(x => x.UserName)
            .Paginar(paginacionDTO)
            .Select(u => new UsuarioDTO 
            { 
                Id = u.Id,
                Email = u.Email!
            }).AsNoTracking().ToListAsync();

        var conteo = await context.Users.CountAsync();
        var respuesta = new ResultadoPaginadoDTO<UsuarioDTO>
        {
            CantidadTotalRegistros = conteo,
            Elemento = elementos
        };
        return respuesta;
    }

    public async Task<bool> RemoverAdmin(string email)
    {
        using var context = dbFactory.CreateDbContext();
        var usuario = await userManager.FindByEmailAsync(email);

        if(usuario is null)
        {
            return false;
        }
        await userManager.RemoveFromRoleAsync(usuario, "administrador");
        await userManager.UpdateSecurityStampAsync(usuario);//se actualiza el security stamp para invalidar las cookies de autenticacion del usuario y forzar a que se vuelva a autenticar con los nuevos roles asignados.
        return true;
    }
}
