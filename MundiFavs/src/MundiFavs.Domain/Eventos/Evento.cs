using System;
using Volo.Abp.Domain.Entities;

namespace MundiFavs.Eventos;

public class Evento : AggregateRoot<Guid>
{
    public string ExternalId { get; private set; }
    public string Nombre { get; set; }
    public string Url { get; set; }
    public DateTime FechaInicio { get; set; }
    public string ImagenUrl { get; set; }

    // Relación con el Destino (Mapeo por ID según DDD)
    public Guid DestinoId { get; set; }

    protected Evento() { } // Requerido por EF Core

    public Evento(Guid id, string externalId, Guid destinoId, string nombre) : base(id)
    {
        ExternalId = externalId;
        DestinoId = destinoId;
        Nombre = nombre;
    }
}