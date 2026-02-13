using System;
using Volo.Abp.Application.Dtos;

namespace MundiFavs.Eventos
{
    public class EventoDto : AuditedEntityDto<Guid>
    {
        public Guid DestinoId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class CreateEventoDto
    {
        public Guid DestinoId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}