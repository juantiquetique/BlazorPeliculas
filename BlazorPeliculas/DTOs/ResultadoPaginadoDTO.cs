namespace BlazorPeliculas.DTOs;

public class ResultadoPaginadoDTO<T>
{
    public IEnumerable<T> Elemento { get; set; } = [];
    public int CantidadTotalRegistros { get; set; }
}
