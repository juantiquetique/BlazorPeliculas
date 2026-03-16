using BlazorPeliculas.Entidades;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorPeliculas.Components.Pages;

public partial class Counter (ServicioSingleton singleton, ServicioTransient transient, ServicioScoped scoped2, IJSRuntime JS)
{
    private int currentCount = 0;
    private static int currentCountStatic = 0;

    IJSObjectReference? moduloCounter;

    [JSInvokable]
    public async Task IncrementCount()
    {
        moduloCounter = await JS.InvokeAsync<IJSObjectReference>("import", "./js/counter.js");

        await moduloCounter.InvokeVoidAsync("mostrarAlerta", "Vas a incrementar el contador ");

        currentCount++;
        currentCountStatic = currentCount;
        transient.valor = currentCount;
        singleton.valor = currentCount;
        scoped2.valor = currentCount;
        await JS.InvokeVoidAsync("obtenerCurrentCount");
    }

    //Ejemplos de metodo de instancia
    public async Task IncrementCountJavaScript()
    {
        await JS.InvokeVoidAsync("invocarIncrementCount", DotNetObjectReference.Create(this));
    }

    //ejemplo de metodo estatico
    [JSInvokable]
    public static Task<int> ObtenerCurrentCount()
    {
        return Task.FromResult(currentCountStatic);
    }
}
