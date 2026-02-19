using System;
using System.Collections.Generic; // Necesario para List<>
using System.Threading.Tasks;     // Necesario para Task
using Volo.Abp.Application.Services; // Necesario para IApplicationService

namespace MundiFavs.Notificaciones
{
    public interface INotificacionAppService : IApplicationService
    {
        // 1. Método interno: Para que DestinoAppService pueda crear notificaciones
        Task CrearNotificacionInternaAsync(Guid usuarioId, string tituloDestino, string cambioDetectado);

        // 2. Método para Frontend: Para ver el historial
        Task<List<NotificacionDto>> GetMisNotificacionesAsync();

        // 3. Método para Frontend: Para marcar como leída
        Task MarcarComoLeidaAsync(Guid id);
    }
}