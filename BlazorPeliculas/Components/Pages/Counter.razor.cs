using BlazorPeliculas.Entidades;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorPeliculas.Components.Pages;

public partial class Counter (ServicioSingleton singleton, ServicioTransient transient, ServicioScoped scoped2)
{
    private int currentCount = 0;
    private void IncrementCount()
    {
        currentCount++;
        transient.valor = currentCount;
        singleton.valor = currentCount;
        scoped2.valor = currentCount;
    }
}
