using Microsoft.JSInterop;

namespace BlazorPeliculas.Client.Utilidades;


public static class IJSRuntimeExtensionMethods
{
    public async static ValueTask<bool> Confirm(this IJSRuntime JS, string mensaje)
    {
               return await JS.InvokeAsync<bool>("confirm", mensaje);
    }
    public async static ValueTask MostrarAlertaExitosa(this IJSRuntime JS, string titulo, string cuerpo)
    {
        //mostrarAlerta: es la función JavaScript que se encuentra en wwwroot/js/utilidades.js, la cual utiliza la librería SweetAlert2 para mostrar una alerta personalizada.
        //success: es el icicono que se mostrará en la alerta, indicando que es una alerta de éxito.
        await JS.InvokeVoidAsync("mostrarAlerta", titulo, cuerpo, "success");
    }
    public async static ValueTask MostrarAlertaError(this IJSRuntime JS, string titulo, string cuerpo)
    {
        //mostrarAlerta: es la función JavaScript que se encuentra en wwwroot/js/utilidades.js, la cual utiliza la librería SweetAlert2 para mostrar una alerta personalizada.
        //error: es el icicono que se mostrará en la alerta, indicando que es una alerta de error.
        await JS.InvokeVoidAsync("mostrarAlerta", titulo, cuerpo, "error");
    }
}
