using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Volo.Abp.Guids; 
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
            Timer.Period = 2 * 60 * 60 * 1000;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
           

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var destinoRepository = scope.ServiceProvider.GetRequiredService<IRepository<Destino, Guid>>();
                var eventoService = scope.ServiceProvider.GetRequiredService<IEventoAppService>();
                var eventoRepository = scope.ServiceProvider.GetRequiredService<IRepository<Evento, Guid>>();

                var notificacionRepository = scope.ServiceProvider.GetRequiredService<IRepository<Notificacion, Guid>>();
                var guidGenerator = scope.ServiceProvider.GetRequiredService<IGuidGenerator>(); 

                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

                using (var uow = uowManager.Begin(requiresNew: true, isTransactional: true))
                {
                    var todosLosDestinos = await destinoRepository.GetListAsync();

                    if (!todosLosDestinos.Any())
                    {
                        Logger.LogWarning("⚠️ No hay destinos guardados. El worker no tiene nada que buscar.");
                        return;
                    }

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
                                        var yaExiste = await eventoRepository.AnyAsync(x =>
                                            x.ExternalId == eventoDto.ExternalId &&
                                            x.DestinoId == destinoUsuario.Id);

                                        if (!yaExiste)
                                        {
                                            // 1. Guardar Evento 
                                            eventoDto.DestinoId = destinoUsuario.Id;
                                            await eventoService.GuardarEventoAsync(eventoDto);

                                            // 2. Crear Notificación
                                            if (destinoUsuario.CreatorId.HasValue)
                                            {
                                                var usuarioId = destinoUsuario.CreatorId.Value;
                                                var mensaje = $"¡Nuevo evento en {destinoUsuario.Ciudad}: {eventoDto.Nombre}!";

                                                
                                                
                                                var nuevaNotificacion = new Notificacion(
                                                    guidGenerator.Create(),
                                                    usuarioId,
                                                    destinoUsuario.Ciudad,
                                                    mensaje
                                                );

                                                await notificacionRepository.InsertAsync(nuevaNotificacion);

                                                Logger.LogInformation($"✅ GUARDADO: Notificación en BD para usuario {usuarioId}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"❌ ERROR procesando la ciudad {ciudadRepresentante.Ciudad}: {ex.Message}");
                        }
                    }

                    await uow.CompleteAsync();
                }
            }
            Logger.LogInformation("🛑 WORKER FINALIZADO.");
            Logger.LogInformation("=========================================");
        }
    }
}