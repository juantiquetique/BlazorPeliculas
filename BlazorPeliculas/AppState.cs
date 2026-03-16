using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace BlazorPeliculas;

public class AppState
{
    public string Color { get; set; } = "red"; 
}


public class AppStateService
{
    public AppState AppState { get; set; } = new();

    //una fuente de un valor de cascada
    public CascadingValueSource<AppState> Source { get; }

    //un constructor que inicializa la fuente de valor de cascada con el estado de la aplicación
    //el isFixed: false indica que el valor de cascada no es fijo y puede cambiar, lo que permite
    //a los componentes que lo consumen actualizarse cuando el estado de la aplicación cambia
    public AppStateService()
    {
        Source = new CascadingValueSource<AppState>(AppState, isFixed: false);
    }

    //un método que notifica a los componentes que el estado de la aplicación ha cambiado
    public Task NotifyChangedAsync() => Source.NotifyChangedAsync();
}