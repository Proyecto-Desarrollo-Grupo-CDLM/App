using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MundiFavs.Experiencias
{
    public class Experiencia : FullAuditedAggregateRoot<Guid>
    {
        public Guid DestinoId { get; private set; }
        public Guid UserdId { get; set; }
        public string Comentario { get; set; }
        public Valoracion Valoracion { get; set; }
        public string Etiquetas { get; set; }
        public DateTime FechaExperiencia { get; set; }

        private Experiencia() { }

        public Experiencia(Guid id, Guid userId, Guid destinoId, string comentario, Valoracion valoracion, string etiquetas, DateTime fechaExperiencia)
            : base(id)
        {
            UserdId = userId;
            DestinoId = destinoId;
            Comentario = comentario;
            Valoracion = valoracion;
            Etiquetas = etiquetas;
            FechaExperiencia = fechaExperiencia;
        }
    };

    public enum Valoracion
    {
        Neutral = 0,
        Positiva = 1,
        Negativa = 2
    }
}