namespace BlazorPeliculas.DTOs
{
    public class ParametrosBusquedaPeliculaDTO
    {
        public int Pagina { get; set; } = 1;
        public int RestrosPorPagina { get; set; } = 10;
        public PaginacionDTO PaginacionDTO
        { 
            get
            {
                return new PaginacionDTO { Pagina = Pagina, RegistrosPorPagina = RestrosPorPagina };
            } 
        }

        public string? Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCartelera { get; set; }
        public bool Estrenos { get; set; }
        public bool MasVotadas { get; set; }
    }
}