using BlazorPeliculas.Entidades;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorPeliculas.Components.Pages;

public partial class Counter (ServicioSingleton singleton, ServicioTransient transient, ServicioScoped scoped2, IJSRuntime JS)
{
    private int currentCount = 0;
    private static int currentCountStatic = 0;
    private async Task IncrementCount()
    {
        currentCount++;
        currentCountStatic = currentCount;
        transient.valor = currentCount;
        singleton.valor = currentCount;
        scoped2.valor = currentCount;
        await JS.InvokeVoidAsync("obtenerCurrentCount");
    }

    [JSInvokable]
    public static Task<int> ObtenerCurrentCount()
    {
        return Task.FromResult(currentCountStatic);
    }
}
