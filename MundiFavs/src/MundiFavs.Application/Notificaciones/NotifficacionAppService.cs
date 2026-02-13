using Microsoft.AspNetCore.Authorization;
using MundiFavs.Notificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace MundiFavs.Notificaciones
{
    
    [Authorize] 
    public class NotificacionAppService : ApplicationService, INotificacionAppService
    {
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;

        public NotificacionAppService(IRepository<Notificacion, Guid> notificacionRepository)
        {
            _notificacionRepository = notificacionRepository;
        }

        
        public async Task CrearNotificacionInternaAsync(Guid usuarioId, string tituloDestino, string cambioDetectado)
        {
            

            var notificacion = new Notificacion(
                GuidGenerator.Create(),
                usuarioId,
                tituloDestino,
                cambioDetectado
            );

            await _notificacionRepository.InsertAsync(notificacion, autoSave: true);
        }

       
        public async Task<List<NotificacionDto>> GetMisNotificacionesAsync()
        {
            if (CurrentUser.Id == null)
            {
                return new List<NotificacionDto>();
            }

            // Filtramos solo las notificaciones del usuario actual
            var notificaciones = await _notificacionRepository.GetListAsync(n => n.UsuarioId == CurrentUser.Id);

            // Convertimos a DTO para devolver al frontend
            return notificaciones
                .OrderByDescending(n => n.Fecha)
                .ThenByDescending(n => n.Hora)
                .Select(n => new NotificacionDto
                {
                    Id = n.Id,
                    TituloDestino = n.TituloDestino,
                    CambioDetectado = n.CambioDetectado,
                    Leida = n.Leida,
                    Fecha = n.Fecha,
                    Hora = n.Hora
                }).ToList();
        }

        
        public async Task MarcarComoLeidaAsync(Guid id)
        {
            var notificacion = await _notificacionRepository.GetAsync(id);

            // Seguridad: Verificar que la notificación sea del usuario que intenta modificarla
            if (notificacion.UsuarioId != CurrentUser.Id)
            {
                throw new UnauthorizedAccessException("No tienes permiso para modificar esta notificación.");
            }

            notificacion.MarcarComoLeida(); // Usamos el método de dominio si existe, o notificacion.Leida = true;

            await _notificacionRepository.UpdateAsync(notificacion);
        }
    }

   
}