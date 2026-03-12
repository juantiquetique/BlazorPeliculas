namespace BlazorPeliculas;

//tiempos de vida de los servicios

//se crea una unica instancia del servicio, y se comparte en toda la aplicación hasta que se reinicie la aplicación
// todo el mundo comparte la misma instancia de la clase
public class ServicioSingleton
{
    public int valor { get; set; }
}

//se crea una nueva instancia del servicio cada vez que se inyecta, es decir, cada vez que se solicita el servicio, se crea una nueva instancia de la clase
public class ServicioTransient
{
    public int valor { get; set; }
}

//se encuentra en un servicio en un contexto determinado, en ambientes web, tipicamente ese contexto es una peticion HTTP
//se crea una nueva instancia del servicio para cada solicitud HTTP,
//y esa instancia se comparte durante toda la solicitud, es decir, si durante una solicitud HTTP se solicita el servicio varias veces,
//se obtiene la misma instancia de la clase, pero si se realiza otra solicitud HTTP, se crea una nueva instancia de la clase
public class ServicioScoped
{
    public int valor { get; set; }
}