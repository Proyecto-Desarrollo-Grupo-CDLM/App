using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MundiFavs.Eventos
{
    public class Evento : AuditedEntity<Guid>
    {
        public Guid DestinoId { get; set; } // Relación con el Destino
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        protected Evento() { }

        public Evento(Guid id, Guid destinoId, string titulo, string descripcion, DateTime fechaInicio, DateTime fechaFin)
            : base(id)
        {
            DestinoId = destinoId;
            Titulo = titulo;
            Descripcion = descripcion;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }
    }
}