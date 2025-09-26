using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;


namespace MundiFavs.Destinos;
public class Destino : AuditedAggregateRoot<Guid>
{
      public string Nombre { get; private set; }
    public string Pais { get; private set; }
    public string Ciudad { get; private set; }
    public int Poblacion { get; private set; }
    public Coordenadas Ubicacion { get; private set; }
    public Uri ImageUrl { get; private set; }
    private Destino() { }

    public Destino(
        Guid id,
        string nombre,
        string pais,
        string ciudad,
        int poblacion,
        Coordenadas ubicacion,
        Uri imageUrl)
        : base(id)
    {
        Nombre = nombre;
        Pais = pais;
        Ciudad = ciudad;
        Poblacion = poblacion;
        Ubicacion = ubicacion;
        ImageUrl = imageUrl;
    }
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


