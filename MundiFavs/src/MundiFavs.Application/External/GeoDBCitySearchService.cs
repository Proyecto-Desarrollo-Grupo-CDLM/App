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
                            // --- NUEVO: Mapeamos el código de país para el frontend ---
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
    }
}