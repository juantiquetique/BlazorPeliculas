using BlazorPeliculas.Constantes;
using BlazorPeliculas.Datos;
using BlazorPeliculas.Client.DTOs;
using BlazorPeliculas.Client.Entidades;
using BlazorPeliculas.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorPeliculas.Entidades;
using BlazorPeliculas.Client.Servicios;

namespace BlazorPeliculas.Servicios;
//el usermanager es el que nos permite realizar operaciones sobre los usuarios en este caso vamos agregar y remover el rol administrador aun usuario, el dbcontextfactory es para crear instancias del contexto de la base de datos.
public class ServicioSeguridad(IDbContextFactory<ApplicationDbContext> dbFactory,
    UserManager<ApplicationUser>userManager, IHttpContextAccessor httpContextAccessor,
    IAuthorizationService authorizacionService) 
                : IServicioSeguridad
{
    public async Task<ResultadoAccion> HacerAdmin(string email)
    {
        var usuarioLogueado = httpContextAccessor.HttpContext!.User; //obtenemos el usuario logueado a traves del httpcontextaccessor.
        var resultado = await authorizacionService.AuthorizeAsync(usuarioLogueado, "PuedeEditarRolesDB");

        if(!resultado.Succeeded)
        {
            return ResultadoAccion.NoTienePermiso;
        }

        using var context = dbFactory.CreateDbContext();
        var usuario = await userManager.FindByEmailAsync(email);

        if(usuario is null)
        {
            return ResultadoAccion.NoEncontrado;
        }
        await userManager.AddToRoleAsync(usuario, Roles.ROL_ADMIN);
        await userManager.UpdateSecurityStampAsync(usuario);//se actualiza el security stamp para invalidar las cookies de autenticacion del usuario y forzar a que se vuelva a autenticar con los nuevos roles asignados.
        return ResultadoAccion.Exitoso;
    }

    //private async Task<bool> ValidarUsuarioEsAdmin()
    //{
    //    var usuarioLogueado = httpContextAccessor.HttpContext!.User; //obtenemos el usuario logueado a traves del httpcontextaccessor.
    //    var usuarioLogueadoApplicationUser = 
    //        await userManager.FindByEmailAsync(usuarioLogueado.Identity!.Name!); //busca por el email del usuario logueado en la base de datos y devuelve el objeto ApplicationUser correspondiente.
        
    //    //con esto voy a la base de datos y verifico si el usuario logueado tiene el rol de administrador, si es asi devuelve true, sino devuelve false.
    //    return await userManager.IsInRoleAsync(usuarioLogueadoApplicationUser!, Roles.ROL_ADMIN);
    //}

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

    public async Task<ResultadoAccion> RemoverAdmin(string email)
    {
        var usuarioLogueado = httpContextAccessor.HttpContext!.User; //obtenemos el usuario logueado a traves del httpcontextaccessor.
        var resultado = await authorizacionService.AuthorizeAsync(usuarioLogueado, "PuedeEditarRolesDB");

        if (!resultado.Succeeded)
        {
            return ResultadoAccion.NoTienePermiso;
        }

        using var context = dbFactory.CreateDbContext();
        var usuario = await userManager.FindByEmailAsync(email);

        if(usuario is null)
        {
            return ResultadoAccion.NoEncontrado;
        }
        await userManager.RemoveFromRoleAsync(usuario, Roles.ROL_ADMIN);
        await userManager.UpdateSecurityStampAsync(usuario);//se actualiza el security stamp para invalidar las cookies de autenticacion del usuario y forzar a que se vuelva a autenticar con los nuevos roles asignados.
        return ResultadoAccion.Exitoso;
    }
}
