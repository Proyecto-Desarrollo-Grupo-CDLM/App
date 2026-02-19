using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace MundiFavs.Destinos
{
    public class ComentarioDto : EntityDto<Guid>
    {
        public int Estrellas { get; set; }
        public  string Comentario { get; set; }

        public string AutorNombre { get; set; } // Opcional: si quieres mostrar quién comentó
        public Guid UserId { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class DestinoComentariosDto
    {
        public Guid DestinoId { get; set; }

        public string NombreDestino { get; set; } = string.Empty;

        public double PuntuacionPromedio { get; set; }

        public int TotalCalificaciones { get; set; }

        public List<ComentarioDto> Comentarios { get; set; } = new();
    }
}