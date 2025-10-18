var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews(); // Controladores + vistas
builder.Services.AddRazorPages();            // Páginas Razor si las necesitas

// Swagger (opcional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Permitir servir archivos estáticos del cliente Blazor
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

// Mapear rutas del servidor
app.MapRazorPages();
app.MapControllers();

// ✅ Este es el paso clave: si no hay ruta, sirve el index.html del cliente
app.MapFallbackToFile("index.html");

app.Run();
