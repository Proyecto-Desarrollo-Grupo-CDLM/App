using Microsoft.AspNetCore.Authorization;
using MundiFavs.CitySearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace MundiFavs.External.CitySearch
{
    [Authorize]
    public class GeoDbCitySearchService : ICitySearchService
    {
        private const string ApiKey = "986ccbd81fmsh61e9796386ee6f0p144224jsn0008c9636ca0";
        private const string BaseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";
        private const string Host = "wft-geo-db.p.rapidapi.com";
        private readonly HttpClient _httpClient;

        public GeoDbCitySearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<CityDetailDto> GetCityDetailById(CityDetailRequestDto input)
        {
            // 1. Validar la entrada (aunque el AppService también lo hará)
            if (string.IsNullOrWhiteSpace(input?.CityId))
            {
                throw new UserFriendlyException("El ID de la ciudad es requerido para obtener detalles.");
            }

            // Construir la URL para detalles: /cities/{cityId}
            var url = $"{BaseUrl}/cities/{Uri.EscapeDataString(input.CityId)}";

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("X-RapidAPI-Key", ApiKey);
            httpRequest.Headers.Add("X-RapidAPI-Host", Host);

            try
            {
                var response = await _httpClient.SendAsync(httpRequest);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Usar la excepción estándar de ABP para no encontrado
                    throw new EntityNotFoundException($"Ciudad con ID '{input.CityId}' no encontrada en la API externa.");
                }

                response.EnsureSuccessStatusCode(); // Lanza excepción para otros códigos de error 4xx/5xx

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // El endpoint de detalle de GeoDB envuelve la respuesta en 'data'
                var geoDbDetailResponse = JsonSerializer.Deserialize<GeoDbCityDetailResponse>(json, options);

                if (geoDbDetailResponse?.Data == null)
                {
                    throw new UserFriendlyException($"Respuesta inválida de la API externa para el ID: {input.CityId}");
                }

                // Mapeo manual del DTO de la API al DTO de la aplicación
                return MapToCityDetailDto(geoDbDetailResponse.Data);
            }
            catch (Exception ex) when (!(ex is EntityNotFoundException))
            {
                // Manejo de errores de red o deserialización
                throw new UserFriendlyException($"Error al obtener detalles de la ciudad: {ex.Message}");
            }
        }

       
        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            var result = new CitySearchResultDto();

            // --- CONSTRUCCIÓN DE URL CON FILTROS (Operación 3.2) ---
            var urlBuilder = new StringBuilder($"{BaseUrl}/cities?limit=10");

            // 1. Filtro por Nombre
            if (!string.IsNullOrWhiteSpace(request?.NombreCiudad))
            {
                urlBuilder.Append($"&namePrefix={Uri.EscapeDataString(request.NombreCiudad)}");
            }

            // 2. Filtro por País
            if (!string.IsNullOrWhiteSpace(request?.CountryCode))
            {
                urlBuilder.Append($"&countryIds={request.CountryCode}");
            }

            // 3. Filtro por Población
            if (request?.MinPopulation.HasValue == true)
            {
                urlBuilder.Append($"&minPopulation={request.MinPopulation}");
            }

            // Ordenar por población descendente
            urlBuilder.Append("&sort=-population");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, urlBuilder.ToString());
            httpRequest.Headers.Add("X-RapidAPI-Key", ApiKey);
            httpRequest.Headers.Add("X-RapidAPI-Host", Host);

            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                if (!response.IsSuccessStatusCode)
                    return result;

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var geoDbResponse = JsonSerializer.Deserialize<GeoDbCitiesResponse>(json, options);

                if (geoDbResponse?.Data != null)
                {
                    foreach (var city in geoDbResponse.Data)
                    {
                        result.CityNames.Add(new CiudadDto
                        {
                            NombreCiudad = city.NombreCiudad,
                            Pais = city.Pais,
                            Region = city.Region,
                            Id = city.Id.ToString(),
                            CountryCode = city.CountryCode
                        });
                    }
                }
            }
            catch
            {
                // Manejo de error silencioso
            }
            return result;
        }


        // -----------------------------------------------------------------------------------
        // ⚠️ DTOs de Infraestructura (Modelando la respuesta de GeoDB)
        // -----------------------------------------------------------------------------------

        // DTO para el endpoint de detalle (/cities/{cityId})
        private class GeoDbCityDetailResponse
        {
            [JsonPropertyName("data")]
            public GeoDbCityDetailData Data { get; set; }
        }

        // DTO de datos para el detalle, incluyendo Latitud, Longitud, Población y Huso Horario
        private class GeoDbCityDetailData
        {
            // Mapeamos a los nombres de la API GeoDB
            [JsonPropertyName("wikiDataId")]
            public string WikiDataId { get; set; } // Usamos este ID para CityDetailDto.Id

            [JsonPropertyName("name")]
            public string NombreCiudad { get; set; }

            [JsonPropertyName("country")]
            public string Pais { get; set; }

            [JsonPropertyName("region")]
            public string Region { get; set; }

            [JsonPropertyName("latitude")]
            public decimal Latitud { get; set; }

            [JsonPropertyName("longitude")]
            public decimal Longitud { get; set; }

            [JsonPropertyName("population")]
            public long Poblacion { get; set; }

            [JsonPropertyName("timezone")]
            public string Timezone { get; set; }

            // Podrías reutilizar GeoDbCity para los campos comunes si lo deseas, pero 
            // definir un DTO específico para el detalle es más seguro.
        }
        private class GeoDbCitiesResponse
        {
            [JsonPropertyName("data")]
            public List<GeoDbCity> Data { get; set; }
        }

        private class GeoDbCity
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string NombreCiudad { get; set; }

            [JsonPropertyName("country")]
            public string Pais { get; set; }

            [JsonPropertyName("region")]
            public string Region { get; set; }

            // --- NUEVO CAMPO NECESARIO PARA EL FILTRO ---
            [JsonPropertyName("countryCode")]
            public string CountryCode { get; set; }

        }
        private CityDetailDto MapToCityDetailDto(GeoDbCityDetailData data)
        {
            return new CityDetailDto
            {
                Id = data.WikiDataId, // Usamos wikiDataId como identificador único
                NombreCiudad = data.NombreCiudad,
                Pais = data.Pais,
                Region = data.Region,
                Latitud = data.Latitud,
                Longitud = data.Longitud,
                Poblacion = data.Poblacion,
                UtcOffset = data.Timezone
            };
        }
    }
}