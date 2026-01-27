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

    // --- 1. NUEVA PROPIEDAD: Aquí se guardará el promedio (ej: 4.5) ---
    public double PuntuacionPromedio { get; private set; }

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

        // --- 2. INICIALIZACIÓN: Al crear un destino nuevo, nace con 0 estrellas ---
        PuntuacionPromedio = 0;
    }

    // --- 3. MÉTODO PARA ACTUALIZAR: El servicio llamará a esto para cambiar el valor ---
    public void SetPuntuacion(double nuevaPuntuacion)
    {
        PuntuacionPromedio = nuevaPuntuacion;
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