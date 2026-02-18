using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MundiFavs.Destinos; // Asegúrate de que este namespace sea correcto para tu Entidad Destino
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading; // Necesario para AbpAsyncTimer
using Microsoft.AspNetCore.SignalR;
//using Volo.Abp.AspNetCore.SignalR;
using Abp.AspNetCore.SignalR.Hubs;

namespace MundiFavs.Eventos
{
    public class EventSyncWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        // 👇 CAMBIO CLAVE: Usamos 'AbpAsyncTimer' en lugar de 'AbpTimer'
        public EventSyncWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            // Configuración: Ejecutar cada 6 horas
            // 6 horas * 60 min * 60 seg * 1000 ms
            Timer.Period = 6 * 60 * 60 * 1000;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogInformation("🔄 Iniciando Sincronización...");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var destinoRepository = scope.ServiceProvider.GetRequiredService<IRepository<Destino, Guid>>();
                var eventoService = scope.ServiceProvider.GetRequiredService<IEventoAppService>();
                var eventoRepository = scope.ServiceProvider.GetRequiredService<IRepository<Evento, Guid>>();

                // 👇 1. OBTENEMOS EL HUB DE SIGNALR
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<AbpCommonHub>>();

                var destinos = await destinoRepository.GetListAsync();

                foreach (var destino in destinos)
                {
                    try
                    {
                        var eventosEncontrados = await eventoService.BuscarEnTicketmasterAsync(destino.Ciudad);

                        if (eventosEncontrados != null)
                        {
                            foreach (var eventoDto in eventosEncontrados)
                            {
                                var existe = await eventoRepository.AnyAsync(x => x.ExternalId == eventoDto.ExternalId);

                                if (!existe)
                                {
                                    eventoDto.DestinoId = destino.Id;
                                    await eventoService.GuardarEventoAsync(eventoDto);

                                    Logger.LogInformation($"✅ Nuevo evento: {eventoDto.Nombre}");

                                    // 👇 2. ENVIAMOS LA NOTIFICACIÓN REAL
                                    var mensaje = $"¡Nuevo evento en {destino.Ciudad}: {eventoDto.Nombre}!";

                                    // 'RecibirNotificacion' debe coincidir con el nombre en Angular
                                    await hubContext.Clients.All.SendAsync("RecibirNotificacion", mensaje, "Exito");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"❌ Error en {destino.Ciudad}: {ex.Message}");
                    }
                }
            }
        }
    }
}