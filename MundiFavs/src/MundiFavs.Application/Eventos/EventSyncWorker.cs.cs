using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;
using MundiFavs.Destinos;
using MundiFavs.Notificaciones; 

namespace MundiFavs.Eventos
{
    public class EventSyncWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventSyncWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            Timer.Period = 60 * 1000; // 60 segundos para pruebas
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogInformation("🔄 Worker: Buscando eventos...");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var destinoRepository = scope.ServiceProvider.GetRequiredService<IRepository<Destino, Guid>>();
                var eventoService = scope.ServiceProvider.GetRequiredService<IEventoAppService>();
                var eventoRepository = scope.ServiceProvider.GetRequiredService<IRepository<Evento, Guid>>();

                // 👇 Solo inyectamos el servicio de notificaciones (Base de Datos)
                var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionAppService>();

                var todosLosDestinos = await destinoRepository.GetListAsync();

                // Agrupamos por ciudad para no saturar la API externa
                var ciudadesUnicas = todosLosDestinos.GroupBy(d => d.Ciudad).Select(g => g.First()).ToList();

                foreach (var ciudadRepresentante in ciudadesUnicas)
                {
                    try
                    {
                        var eventosEncontrados = await eventoService.BuscarEnTicketmasterAsync(ciudadRepresentante.Ciudad);

                        if (eventosEncontrados != null && eventosEncontrados.Any())
                        {
                            var destinosAfectados = todosLosDestinos
                                .Where(d => d.Ciudad == ciudadRepresentante.Ciudad)
                                .ToList();

                            foreach (var eventoDto in eventosEncontrados)
                            {
                                foreach (var destinoUsuario in destinosAfectados)
                                {
                                    // Verificamos si ya existe para este usuario específico
                                    var yaExiste = await eventoRepository.AnyAsync(x =>
                                        x.ExternalId == eventoDto.ExternalId &&
                                        x.DestinoId == destinoUsuario.Id);

                                    if (!yaExiste)
                                    {
                                        // 1. Guardar Evento
                                        eventoDto.DestinoId = destinoUsuario.Id;
                                        await eventoService.GuardarEventoAsync(eventoDto);

                                        // 2. Crear Notificación en BD (Solo si tiene dueño)
                                        if (destinoUsuario.CreatorId.HasValue)
                                        {
                                            var usuarioId = destinoUsuario.CreatorId.Value;
                                            var mensaje = $"¡Nuevo evento en {destinoUsuario.Ciudad}: {eventoDto.Nombre}!";

                                            // Guardamos en la tabla AppNotificaciones
                                            await notificacionService.CrearNotificacionInternaAsync(usuarioId, destinoUsuario.Ciudad, mensaje);

                                            Logger.LogInformation($"🔔 Notificación guardada para usuario {usuarioId}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"❌ Error procesando {ciudadRepresentante.Ciudad}: {ex.Message}");
                    }
                }
            }
        }
    }
}