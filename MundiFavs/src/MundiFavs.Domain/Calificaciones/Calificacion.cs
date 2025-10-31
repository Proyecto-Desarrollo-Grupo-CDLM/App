using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Users;
using Volo.Abp.Auditing;




namespace MundiFavs.Calificaciones
{
    public class Calificacion : AuditedAggregateRoot<Guid> , IUserOwned
    {
        public int Estrellas { get; private set; }

        public string Comentario { get; private set; }
       
        public Destinos.Destino Destino { get; private set; }
        
        public Guid UserId { get;  set; }

        public Guid DestinoId { get; private set; }

        private Calificacion() { }

        public Calificacion(
            Guid id,
            int estrellas,
            string comentario,
            Destinos.Destino destino,
            Guid userId
            )
            : base(id)
        {
            this.Estrellas = estrellas;
            this.Comentario = comentario;
            this.Destino = destino;
            this.UserId = userId;
            this.DestinoId = destino.Id;
        }
    }
}
