using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json; // <--- USAMOS LA LIBRERÍA NATIVA
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MundiFavs.Eventos
{
    public class EventoAppService : ApplicationService, IEventoAppService
    {
        private readonly IRepository<Evento, Guid> _eventoRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public EventoAppService(
            IRepository<Evento, Guid> eventoRepository,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _eventoRepository = eventoRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("api/app/evento/ticketmaster")]
        public async Task<List<EventoDto>> BuscarEnTicketmasterAsync(string ciudad, string keyword = null)
        {
            var apiKey = _configuration["Ticketmaster:ApiKey"];
            var baseUrl = "https://app.ticketmaster.com/discovery/v2/events.json";

            // Construir URL
            var url = $"{baseUrl}?apikey={apiKey}&city={ciudad}&sort=date,asc&size=10";
            if (!string.IsNullOrEmpty(keyword)) url += $"&keyword={keyword}";

            // Crear cliente HTTP
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var listaResultados = new List<EventoDto>();

            // PARSEO CON SYSTEM.TEXT.JSON
            using (JsonDocument doc = JsonDocument.Parse(content))
            {
                var root = doc.RootElement;

                // Verificar si hay eventos ("_embedded" existe)
                if (root.TryGetProperty("_embedded", out JsonElement embedded) &&
                    embedded.TryGetProperty("events", out JsonElement eventsArray))
                {
                    foreach (var item in eventsArray.EnumerateArray())
                    {
                        var dto = new EventoDto
                        {
                            ExternalId = GetSafeString(item, "id"),
                            Nombre = GetSafeString(item, "name"),
                            Url = GetSafeString(item, "url"),
                            DestinoId = Guid.Empty
                        };

                        // -- IMAGEN --
                        if (item.TryGetProperty("images", out JsonElement images) && images.GetArrayLength() > 0)
                        {
                            dto.ImagenUrl = GetSafeString(images[0], "url");
                        }

                        // -- FECHA --
                        if (item.TryGetProperty("dates", out JsonElement dates) &&
                            dates.TryGetProperty("start", out JsonElement start) &&
                            start.TryGetProperty("localDate", out JsonElement localDate))
                        {
                            if (DateTime.TryParse(localDate.GetString(), out DateTime fecha))
                            {
                                dto.FechaInicio = fecha;
                            }
                        }

                        // -- LOCALIDAD --
                        if (item.TryGetProperty("_embedded", out JsonElement itemEmbedded) &&
                            itemEmbedded.TryGetProperty("venues", out JsonElement venues) &&
                            venues.GetArrayLength() > 0)
                        {
                            dto.Localidad = GetSafeString(venues[0], "name");
                        }
                        else
                        {
                            dto.Localidad = ciudad;
                        }

                        listaResultados.Add(dto);
                    }
                }
            }

            return listaResultados;
        }

        public async Task<EventoDto> GuardarEventoAsync(EventoDto input)
        {
            var existe = await _eventoRepository.AnyAsync(x => x.ExternalId == input.ExternalId);
            if (existe)
            {
                throw new UserFriendlyException("¡Este evento ya está guardado en tus favoritos!");
            }

            var nuevoEvento = new Evento(
                GuidGenerator.Create(),
                input.ExternalId,
                input.DestinoId,
                input.Nombre
            );

            nuevoEvento.Url = input.Url;
            nuevoEvento.FechaInicio = input.FechaInicio;
            nuevoEvento.ImagenUrl = input.ImagenUrl;

            await _eventoRepository.InsertAsync(nuevoEvento);
            return input;
        }

        // Helper para leer propiedades JSON de forma segura
        private string GetSafeString(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement prop) &&
                prop.ValueKind == JsonValueKind.String)
            {
                return prop.GetString();
            }
            return string.Empty;
        }
    }
}