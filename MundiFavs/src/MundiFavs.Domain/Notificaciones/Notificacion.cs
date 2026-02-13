using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MundiFavs.Notificaciones
{
    public class Notificacion : AuditedAggregateRoot<Guid>
    {
       
        public Guid UsuarioId { get; set; }

        
        public string TituloDestino { get; set; }   
        public string CambioDetectado { get; set; } 
        public bool Leida { get; set; }
        public DateTime Fecha { get; set; }         
        public TimeSpan Hora { get; set; }          

      
        protected Notificacion() { }

        public Notificacion(Guid id, Guid usuarioId, string tituloDestino, string cambioDetectado)
            : base(id)
        {
            UsuarioId = usuarioId;
            TituloDestino = tituloDestino;
            CambioDetectado = cambioDetectado;

            Leida = false; 

            
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