using System;
using Volo.Abp.Application.Dtos;

namespace MundiFavs.Experiencias
{
    public class GetExperienciasInput : PagedAndSortedResultRequestDto
    {
        public Guid? DestinoId { get; set; }
        public string FiltroTexto { get; set; } // Búsqueda por palabras clave (4.6)
        public Valoracion? Valoracion { get; set; }
    }
}