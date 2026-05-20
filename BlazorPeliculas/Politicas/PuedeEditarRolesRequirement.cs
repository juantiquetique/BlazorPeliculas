using BlazorPeliculas.Constantes;
using BlazorPeliculas.Client.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BlazorPeliculas.Entidades;

namespace BlazorPeliculas.Politicas;

public class PuedeEditarRolesRequirement: IAuthorizationRequirement
{
}

//un handler es la accion que se requiere realizar para poder manejar dicho requerimiento y poder verificar que el usuario cumple o no con las reglas de acceso requeridas
public class PuedeEditarRolesHandler(UserManager<ApplicationUser> userManager) : AuthorizationHandler<PuedeEditarRolesRequirement>
{
    protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
                    PuedeEditarRolesRequirement requirement)
    {
        var usuario = await userManager.GetUserAsync(context.User);//Obtengo el usuario logueado a partir del contexto de autorización

        if (usuario is null)
        {
            return;
        }

        var usuarioEsAdmin = await userManager.IsInRoleAsync(usuario, Roles.ROL_ADMIN);//Verifico si el usuario logueado es administrador

        if (usuarioEsAdmin)
        {
            context.Succeed(requirement);
        }

        var claimsDB = await userManager.GetClaimsAsync(usuario);//Obtengo los claims del usuario logueado
        var tieneClaimSuperAdmin = claimsDB.Any(c => c.Type == "superadmin");//Verifico si el usuario logueado tiene el claim de superadmin

        if(tieneClaimSuperAdmin)
        {
            context.Succeed(requirement);
        }    
    }
}