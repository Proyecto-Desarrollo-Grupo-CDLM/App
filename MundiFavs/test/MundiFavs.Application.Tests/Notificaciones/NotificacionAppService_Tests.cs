using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace MundiFavs.Notificaciones
{
    public abstract class NotificacionAppService_Tests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
         where TStartupModule : IAbpModule
    {
        private readonly INotificacionAppService _notificacionAppService;
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;

        public NotificacionAppService_Tests()
        {
            _notificacionAppService = GetRequiredService<INotificacionAppService>();
            _notificacionRepository = GetRequiredService<IRepository<Notificacion, Guid>>();
        }

        [Fact]
        public async Task CrearNotificacionInternaAsync_Deberia_Guardar_En_Base_De_Datos()
        {
            // Arrange
            var usuarioIdSimulado = Guid.NewGuid();
            var ciudad = "Madrid";
            var mensaje = "¡Coldplay tocará en tu ciudad favorita!";

            // Act
            await _notificacionAppService.CrearNotificacionInternaAsync(usuarioIdSimulado, ciudad, mensaje);

            // Assert
            var notificaciones = await _notificacionRepository.GetListAsync();
            var notificacionGuardada = notificaciones.FirstOrDefault(n => n.UsuarioId == usuarioIdSimulado);

            notificacionGuardada.ShouldNotBeNull();
            notificacionGuardada.TituloDestino.ShouldBe(ciudad);
            notificacionGuardada.CambioDetectado.ShouldBe(mensaje);
            notificacionGuardada.Leida.ShouldBeFalse();
        }
    }
}