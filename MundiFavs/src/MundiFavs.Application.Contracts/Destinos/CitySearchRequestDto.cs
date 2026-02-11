namespace MundiFavs.CitySearch
{
    public class CitySearchRequestDto
    {
        public string NombreCiudad { get; set; }

        /// <summary>
        /// Código ISO-3166 del país (ej: "AR", "US", "ES")
        /// </summary>
        public string? CountryCode { get; set; }

        /// <summary>
        /// Población mínima para filtrar ciudades pequeñas
        /// </summary>
        public int? MinPopulation { get; set; }
    }
}