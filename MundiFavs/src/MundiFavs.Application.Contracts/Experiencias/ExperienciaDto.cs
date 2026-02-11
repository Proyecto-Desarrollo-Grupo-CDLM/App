using MundiFavs.Experiencias;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

public class ExperienciaDto : FullAuditedEntityDto<Guid>
{
    public Guid DestinoId { get; set; }
    public required string Comentario { get; set; }
    public Valoracion Valoracion { get; set; }
    public required string Etiquetas { get; set; }
}

public class CreateUpdateExperienciaDto
{
    public Guid UserId { get; set; }
    public Guid DestinoId { get; set; }
    public required string Comentario { get; set; }
    public Valoracion Valoracion { get; set; }
    public required string Etiquetas { get; set; }
    public DateTime FechaExperiencia { get; set; }
}
