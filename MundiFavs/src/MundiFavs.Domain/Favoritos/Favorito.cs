using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Auditing;

namespace MundiFavs.Favoritos
{
    
    public class Favorito : CreationAuditedEntity<Guid>
    {
        public Guid DestinoId { get; set; }

        protected Favorito() { }

        public Favorito(Guid id, Guid destinoId)
            : base(id)
        {
            DestinoId = destinoId;
        }
    }
}