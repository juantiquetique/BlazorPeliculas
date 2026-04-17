using BlazorPeliculas;
using BlazorPeliculas.Components;
using BlazorPeliculas.Datos;
using BlazorPeliculas.Servicios;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//AddDbContextFactory: es la manera recomendad de usar entity framework core en un modelo como blazor server o que use interactividad con el servidor
builder.Services.AddDbContextFactory<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddScoped<IServicioPeliculas, ServicioPeliculas>();
builder.Services.AddScoped<IServicioGeneros, ServicioGeneros>();
builder.Services.AddScoped<IServicioActores, ServicioActores>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();//se agrega el servicio de almacenamiento de archivos local

builder.Services.AddHttpContextAccessor();

builder.Services.AddMudServices();

var app = builder.Build();

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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
