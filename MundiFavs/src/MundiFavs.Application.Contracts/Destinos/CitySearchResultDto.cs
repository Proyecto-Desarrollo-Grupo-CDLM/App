using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MundiFavs.CitySearch
{
    public class CitySearchResultDto
    {
        
        public List<CiudadDto> CityNames { get; set; } = new ();
    }
}