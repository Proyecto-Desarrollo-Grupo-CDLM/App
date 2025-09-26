using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MundiFavs.Destinos;


public class DestinoDto : AuditedEntityDto<Guid>
{
    public string Nombre { get; set; }
    public string Pais { get; set; }
    public string Ciudad { get; set; }
    public int Poblacion { get; set; }
    public CoordenadasDto Ubicacion { get; set; } 
    public Uri ImageUrl { get; set; }
}

public class CoordenadasDto
{
    public decimal Latitud { get; set; }  
    public decimal Longitud { get; set; } 
}


//setteers publicos