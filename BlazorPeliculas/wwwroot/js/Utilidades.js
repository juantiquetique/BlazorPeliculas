//con metodo statico
function obtenerCurrentCount() {
    //proyecto, método a llamar
    DotNet.invokeMethodAsync("BlazorPeliculas", "ObtenerCurrentCount")
        .then(resultado => {
            console.log(`Conteo desde JS: ${resultado}`);
        })
}

//con metodo de instancia
function invocarIncrementCount(dotnetHelper) {
    dotnetHelper.invokeMethodAsync("IncrementCount");
}