using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MundiFavs.Calificaciones
{
    public class CreateUpdateCalificacionDto
    {
        [Required] // El usuario DEBE especificar qué destino está calificando
        public Guid DestinoId { get; set; }

        [Required]
        [Range(1, 5)] 
        public int Estrellas { get; set; }

        // El comentario es opcional, pero limitamos longitud
        [StringLength(500)]
        public string? Comentario { get; set; }
    }
}
