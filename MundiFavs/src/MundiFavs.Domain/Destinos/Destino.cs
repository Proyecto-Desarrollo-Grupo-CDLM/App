using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;


namespace MundiFavs.Destinos;
public class Destino : AuditedAggregateRoot<Guid>
{
    public string Nombre { get; set; }
    public string Pais { get; set; }
    public string Ciudad { get; set; }
    public int Poblacion { get; set; }
    public Coordenadas Ubicacion { get; private set; }
    public Uri ImageUrl { get; set; }


}
public class Coordenadas
{
    public decimal Latitud { get; private set; }
    public decimal Longitud { get; private set; }

    public Coordenadas(decimal latitud, decimal longitud)
    {
        Latitud = latitud;
        Longitud = longitud;
    }
}
