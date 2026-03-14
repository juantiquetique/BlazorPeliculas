function obtenerCurrentCount() {
    //proyecto, método a llamar
    DotNet.invokeMethodAsync('BlazorPeliculas', 'ObtenerCurrentCount')
        .then(resultado => {
            conle.log(`Conteo desde JS: ${resultado}`)
        })
}