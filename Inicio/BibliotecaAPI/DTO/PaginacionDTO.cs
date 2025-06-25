namespace BibliotecaAPI.DTO
{
    //clase que permite la paginacion de 10 en 10 con un maximo de 40
    public record PaginacionDTO(int Pagina = 1, int RecordsPorPagina = 10) //va a ser inmutable
    {
        private const int CantidadMaximaRecordsPorPagina = 40;
        public int Pagina { get; init; } = Math.Max(1, Pagina); //permite ver desde la 1 en adelante
        //si RecordsPorPagina esta entre 1 y CantidadMaximaRecordsPorPagina lo devuelve sino devuelve :CantidadMaximaRecordsPorPagina o 1 si es menos 
        public int RecordsPorPagina { get; init; } = 
            Math.Clamp(RecordsPorPagina, 1, CantidadMaximaRecordsPorPagina);
    }
}
