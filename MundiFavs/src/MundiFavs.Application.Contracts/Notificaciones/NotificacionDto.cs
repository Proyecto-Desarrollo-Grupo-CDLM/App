using System;
using Volo.Abp.Application.Dtos;

namespace MundiFavs.Notificaciones
{
    public class NotificacionDto : EntityDto<Guid>
    {
        public string TituloDestino { get; set; }
        public string CambioDetectado { get; set; }
        public bool Leida { get; set; }

        // Mapeo exacto con tu diagrama y entidad
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
    }
}