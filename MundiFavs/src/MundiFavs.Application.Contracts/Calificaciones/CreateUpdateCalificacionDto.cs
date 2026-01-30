using System;
using System.ComponentModel.DataAnnotations;

namespace MundiFavs.Calificaciones
{
    public class CreateUpdateCalificacionDto
    {
        [Required]
        public Guid DestinoId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "El valor debe estar entre 1 y 5.")]
        public int Puntuacion { get; set; } // <--- CAMBIO CLAVE: Ahora se llama 'Puntuacion'

        [StringLength(500)]
        public string? Comentario { get; set; }
    }
}

    public class UpdateCalificacionDto
    {
        [Required]
        [Range(1, 5)]
        public int Estrellas { get; set; }
        [StringLength(500)]
        public string? Comentario { get; set; }
    }
}
