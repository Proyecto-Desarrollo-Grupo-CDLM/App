using System;
using Volo.Abp.Domain.Entities;

namespace MundiFavs.Notificaciones
{
    public class PreferenciaNotificacion : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public bool NotificarComentarios { get; set; } = true;
        public bool NotificarEventos { get; set; } = true;

        protected PreferenciaNotificacion() { }

        public PreferenciaNotificacion(Guid id, Guid userId) : base(id)
        {
            UserId = userId;
        }
    }
}