using MundiFavs.CitySearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace MundiFavs.External.CitySearch;

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
 //       if (result.CityNames == null)
 //           result.CityNames = new List<CiudadDto>();
        if (string.IsNullOrWhiteSpace(request?.NombreCiudad))
            return result;

        var url = $"{BaseUrl}/cities?namePrefix={Uri.EscapeDataString(request.NombreCiudad)}&limit=5";
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Add("X-RapidAPI-Key", ApiKey);
        httpRequest.Headers.Add("X-RapidAPI-Host", Host);

        try
        {
            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return result;

            var json = await response.Content.ReadAsStringAsync();
            var geoDbResponse = JsonSerializer.Deserialize<GeoDbCitiesResponse>(json);
            if (geoDbResponse?.Data != null)
            {
                foreach (var city in geoDbResponse.Data)
                {
                    result.CityNames.Add(new CiudadDto
                    {
                        NombreCiudad = city.NombreCiudad,
                        Pais = city.Pais,
                        Region = city.Region,
                        Id = city.Id.ToString()
                    });
                }
            }
        }
        catch
        {
            // Manejo de error: retorna lista vacía
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
    }
}