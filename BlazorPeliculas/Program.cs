using BlazorPeliculas;
using BlazorPeliculas.Components;
using BlazorPeliculas.Components.Account;
using BlazorPeliculas.Constantes;
using BlazorPeliculas.Datos;
using BlazorPeliculas.Entidades;
using BlazorPeliculas.Politicas;
using BlazorPeliculas.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//las siguientes dos lineas se comentan porque ahora nuestro proyecto va poder usar WebAssembly con las otras 4 lineas del builder(cap. 151 curso)
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("PuedeEditarRoles",politica =>
    {
        politica.RequireAssertion(contexto =>
        {
            return contexto.User.IsInRole(Roles.ROL_ADMIN) || 
                    contexto.User.HasClaim(c => c.Type == "superadmin");
        });
    });

    opciones.AddPolicy("PuedeEditarRolesDB", policy =>
    {
        policy.AddRequirements(new PuedeEditarRolesRequirement());
    });
});

builder.Services.AddScoped<IAuthorizationHandler, PuedeEditarRolesHandler>();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

//AddDbContextFactory: es la manera recomendad de usar entity framework core en un modelo como blazor server o que use interactividad con el servidor
builder.Services.AddDbContextFactory<ApplicationDbContext>(opciones => 
opciones.UseSqlServer("name=DefaultConnection")
.UseSeeding((context,_) =>
{
    //aqui manualmente se cre el rol de administrador
    //con el dateseeding nos permite insertar datos a nuestra base de datos cuando son datos de configuracion
    var rolAdmin = "administrador";
    var roles = context.Set<IdentityRole>();//se obtiene el conjunto de roles de la base de datos para verificar si ya existe el rol de administrador
    var existeRolAdmin = roles.Any(r => r.Name == rolAdmin);
    //si no existe el rol de administrador se crea y se guarda en la base de datos
    if (!existeRolAdmin)
    {
        roles.Add(new IdentityRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = rolAdmin,
            NormalizedName = rolAdmin.ToUpperInvariant()
        });
        context.SaveChanges();
    }
}).UseAsyncSeeding(async (context, _, ct) => 
{
    var rolAdmin = "administrador";
    var roles = context.Set<IdentityRole>();
    var existeRolAdmin = await roles.AnyAsync(r => r.Name == rolAdmin, ct);

    if (!existeRolAdmin)
    {
        roles.Add(new IdentityRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = rolAdmin,
            NormalizedName = rolAdmin.ToUpperInvariant()
        });
        await context.SaveChangesAsync(ct);
    }
}));

builder.Services.AddScoped<IServicioPeliculas, ServicioPeliculas>();
builder.Services.AddScoped<IServicioGeneros, ServicioGeneros>();
builder.Services.AddScoped<IServicioActores, ServicioActores>();
builder.Services.AddScoped<IServicioSeguridad, ServicioSeguridad>();
builder.Services.AddScoped<IServicioVotos, ServicioVotos>();
builder.Services.AddTransient<IEmailSender, ServicioCorreos>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();//se agrega el servicio de almacenamiento de archivos local

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMudServices();

var app = builder.Build();

//aqui se ejecuta la migracion de la base de datos al iniciar la aplicacion,
//se crea un scope para obtener el dbcontext y ejecutar la migracion,
//esto es necesario para que la aplicacion pueda crear las tablas en la base de datos si no existen
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if(dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
}

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.UseStaticFiles();//se agrega para que el servidor pueda servir los archivos estáticos como css, js, imágenes, etc localmente

//las siguientes dos lineas se comentan porque ahora nuestro proyecto va poder usar WebAssembly con las otras 4 lineas del builder(cap. 151 curso)
//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorPeliculas.Client._Imports).Assembly);//me permite servir los componentes que se encuentren en el proyecto de client, desde el servidor

app.MapAdditionalIdentityEndpoints();

app.Run();
