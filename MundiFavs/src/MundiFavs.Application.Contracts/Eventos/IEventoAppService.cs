using MundiFavs.Eventos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MundiFavs.Eventos
{
    public interface IEventoAppService : IApplicationService
    {
        // Trae datos vivos de la API externa
        Task<List<EventoDto>> BuscarEnTicketmasterAsync(string ciudad, string keyword = null);

        // Guarda un evento en TU base de datos
        Task<EventoDto> GuardarEventoAsync(EventoDto input);
    }
}