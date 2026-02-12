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
    // -----------------------------------------------------------------------
    // 1. LA INTERFAZ
    // -----------------------------------------------------------------------
    public interface INotificacionAppService : IApplicationService
    {
        // Método interno para usar desde otros servicios (Backend)
        Task CrearNotificacionInternaAsync(Guid usuarioId, string tituloDestino, string cambioDetectado);

        // Métodos para la UI (Frontend)
        Task<List<NotificacionDto>> GetMisNotificacionesAsync();
        Task MarcarComoLeidaAsync(Guid id);
    }

    // -----------------------------------------------------------------------
    // 2. LA IMPLEMENTACIÓN
    // -----------------------------------------------------------------------
    [Authorize] // Requiere estar logueado
    public class NotificacionAppService : ApplicationService, INotificacionAppService
    {
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;

        public NotificacionAppService(IRepository<Notificacion, Guid> notificacionRepository)
        {
            _notificacionRepository = notificacionRepository;
        }

        // IMPLEMENTACIÓN: CrearNotificacionInternaAsync
        public async Task CrearNotificacionInternaAsync(Guid usuarioId, string tituloDestino, string cambioDetectado)
        {
            // --- CORRECCIÓN AQUÍ ---
            // Usamos el constructor que definimos en la Entidad.
            // Pasamos los datos obligatorios entre paréntesis ().
            // La fecha, hora y el estado 'leida' se calculan DENTRO de la entidad automáticamente.

            var notificacion = new Notificacion(
                GuidGenerator.Create(),
                usuarioId,
                tituloDestino,
                cambioDetectado
            );

            await _notificacionRepository.InsertAsync(notificacion, autoSave: true);
        }

        // IMPLEMENTACIÓN: GetMisNotificacionesAsync
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

        // IMPLEMENTACIÓN: MarcarComoLeidaAsync
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

    // -----------------------------------------------------------------------
    // 3. EL DTO
    // -----------------------------------------------------------------------
    public class NotificacionDto : EntityDto<Guid>
    {
        public string TituloDestino { get; set; }
        public string CambioDetectado { get; set; }
        public bool Leida { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
    }
}