namespace MundiFavs.CitySearch
{
    public class CiudadDto
    {
        public string NombreCiudad { get; set; }
        public string Pais { get; set; }

        public string Region { get; set; }
        
        public string Id { get; set; }

        public string CountryCode { get; set; }
    }
    public class CityDetailDto
    {
        public string Id { get; set; } // Opcional, pero útil
        public string NombreCiudad { get; set; }
        public string Pais { get; set; }
        public string Region { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public long Poblacion { get; set; }
        public string UtcOffset { get; set; } 
    }

    public class CityDetailRequestDto
    {
        public string CityId { get; set; }
    }
}