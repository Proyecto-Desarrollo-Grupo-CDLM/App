using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MundiFavs.Notificaciones
{
    public class Notificacion : AuditedAggregateRoot<Guid>
    {
        // Relación con el usuario (necesaria por la línea de asociación en el diagrama)
        public Guid UsuarioId { get; set; }

        // --- Atributos del Diagrama ---
        public string TituloDestino { get; set; }   // Antes: Titulo
        public string CambioDetectado { get; set; } // Antes: Mensaje
        public bool Leida { get; set; }
        public DateTime Fecha { get; set; }         // Diagrama: fecha (date)
        public TimeSpan Hora { get; set; }          // Diagrama: hora (Time)

        // Constructor para EF Core
        protected Notificacion() { }

        public Notificacion(Guid id, Guid usuarioId, string tituloDestino, string cambioDetectado)
            : base(id)
        {
            UsuarioId = usuarioId;
            TituloDestino = tituloDestino;
            CambioDetectado = cambioDetectado;

            Leida = false; // Nace no leída

            // Asignamos fecha y hora actuales al momento de crear
            var ahora = DateTime.Now;
            Fecha = ahora.Date;
            Hora = ahora.TimeOfDay;
        }

        public void MarcarComoLeida()
        {
            Leida = true;
        }
    }
}