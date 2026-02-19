using System;

namespace MundiFavs.Eventos;

public class EventoDto
{
    public string ExternalId { get; set; } // El ID que viene de Ticketmaster (ej: "vvG1HZp6uG7aA")
    public string Nombre { get; set; }
    public string Url { get; set; }
    public DateTime FechaInicio { get; set; }
    public string ImagenUrl { get; set; }
    public string Localidad { get; set; } // Ciudad/Venue
    public Guid DestinoId { get; set; } // FK a tu entidad Destino local
}