namespace BlazorPeliculas.DTOs;
//Se usa un contructor primario
public class PaginacionDTO(int Pagina= 1, int RegistrosPorPagina=10) 
{
    private const int _cantidadMaximaRegistrosPorPagina = 50;
    //el valor debe ser mas grande o igual a 1, si no se asigna el valor de 1
    public int Pagina { get; set; } = Math.Max(1, Pagina);
    //con el math.clamp me dice que los registros por pagina deben ser entre 1 y la cantidad maxima de registros por pagina, si no se asigna el valor de 10
    public int RegistrosPorPagina { get; init; } = 
        Math.Clamp(RegistrosPorPagina,1,_cantidadMaximaRegistrosPorPagina);
}
