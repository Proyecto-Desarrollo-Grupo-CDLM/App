using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Users;
using System.ComponentModel.DataAnnotations; // Añadido para validación de Longitud/Rango (si se requiere)
using Volo.Abp.Domain.Entities; // Añadido para lanzar EntityNotFoundException si se requiere
using Volo.Abp.Data; // Puede ser útil para Invariantes
using Volo.Abp; // Para la clase Check

namespace MundiFavs.Calificaciones
{
    public class Calificacion : AuditedAggregateRoot<Guid>, IUserOwned
    {
        // Propiedades con private set para forzar la actualización a través de métodos de dominio
        [Range(1, 5)] // Aunque es una entidad, la metadata ayuda
        public int Estrellas { get; private set; }

        [StringLength(250)]
        public string? Comentario { get; private set; }

        public Destinos.Destino Destino { get; private set; }

        public Guid UserId { get; set; }

        public Guid DestinoId { get; private set; }

        private Calificacion() { }

    
        public Calificacion(
            Guid id,
            int estrellas,
            string? comentario,
            Destinos.Destino destino,
            Guid userId
            )
            : base(id)
        {
            if (estrellas < 1 || estrellas > 5)
            {
               
                throw new ArgumentOutOfRangeException(nameof(estrellas), "La puntuación debe estar entre 1 y 5.");
            }

            if (comentario != null && comentario.Length > 250)
            {
                throw new ArgumentException("El comentario no puede exceder los 250 caracteres.", nameof(comentario));
            }

            
            this.Estrellas = estrellas;
            this.Comentario = comentario;
            this.UserId = userId;
            this.DestinoId = destino.Id;

        }

        // ----------------------------------------------------------------------------------
        // *** NUEVO MÉTODO DE COMPORTAMIENTO PARA LA FUNCIONALIDAD 5.3 (EDITAR) ***
        // ----------------------------------------------------------------------------------

        
        public void Update(int nuevasEstrellas, string? nuevoComentario)
        {
            if (nuevasEstrellas < 1 || nuevasEstrellas > 5)
            {
                 // Usamos ArgumentOutOfRangeException para el rango.
                 throw new ArgumentOutOfRangeException(nameof(nuevasEstrellas), "La puntuación debe estar entre 1 y 5.");
            }

            // Aplicamos invariantes de dominio para el comentario
            if (nuevoComentario != null && nuevoComentario.Length > 250)
            {
                throw new ArgumentException("El comentario no puede exceder los 250 caracteres.");
            }

            // Asignación de los nuevos valores (usa los setters privados)
            Estrellas = nuevasEstrellas;
            Comentario = nuevoComentario;

            // ABP se encargará de registrar el cambio de hora (LastModificationTime) por AuditedAggregateRoot
        }

        public void ActualizarDatos(int estrellas, string comentario)
        {
            Estrellas = estrellas;
            Comentario = comentario;
        }
    }
}
