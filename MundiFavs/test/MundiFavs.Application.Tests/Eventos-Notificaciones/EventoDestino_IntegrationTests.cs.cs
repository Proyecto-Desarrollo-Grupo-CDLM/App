/*using MundiFavs.Destinos;
using MundiFavs.Eventos;
using MundiFavs.Notificaciones;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;
using System.Collections.Generic; 
using System.Security.Claims; 
using Volo.Abp.Security.Claims; 

namespace MundiFavs
{
    // Heredamos de MundiFavsApplicationTestBase para tener todo el contexto de ABP y EF Core cargado
    public abstract class EventoDestino_IntegrationTests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule

    {
      
        private readonly IDestinoAppService _destinoAppService;
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;
        private readonly IGuidGenerator _guidGenerator;
         private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        // IDs simulados para la prueba
        private Guid _userCreadorId;
        private Guid _userInteresadoId;
        private Guid _userAjenoId;

        public EventoDestino_IntegrationTests()
        {
           
            _destinoAppService = GetRequiredService<IDestinoAppService>();
            _destinoRepository = GetRequiredService<IRepository<Destino, Guid>>();
            _notificacionRepository = GetRequiredService<IRepository<Notificacion, Guid>>();
            _guidGenerator = GetRequiredService<IGuidGenerator>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }

        private IDisposable CambiarUsuario(Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(AbpClaimTypes.UserId, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            return _currentPrincipalAccessor.Change(principal);
        }
        private async Task SeedDatosGeograficosAsync()
        {
            _userCreadorId = Guid.NewGuid();
            _userInteresadoId = Guid.NewGuid();
            _userAjenoId = Guid.NewGuid();

            // 1. Destino del Creador (Paris)
            using (CambiarUsuario(_userCreadorId))
            {
                var destinoCreador = new Destino(
                    _guidGenerator.Create(),
                    "Torre Eiffel",
                    "Francia",
                    "Paris",
                    2000000,
                    new Coordenadas(48.85m, 2.35m),
                    new Uri("http://dummy-image.com")
                );

                // Seteamos el ID Externo (Simulando GeoDB id para Paris)
                destinoCreador.SetExternalId("Q90");

                await _destinoRepository.InsertAsync(destinoCreador, autoSave: true);
            }

            // 2. Destino del Interesado (Paris - Mismo lugar físico)
            using (CambiarUsuario(_userInteresadoId))
            {
                var destinoInteresado = new Destino(
                    _guidGenerator.Create(),
                    "Torre Eiffel",
                    "Francia",
                    "Paris",
                    2000000,
                    new Coordenadas(48.85m, 2.35m),
                    new Uri("http://dummy-image-2.com")
                );

                // Usamos el MISMO ExternalId que el creador para consistencia
                destinoInteresado.SetExternalId("Q90");

                await _destinoRepository.InsertAsync(destinoInteresado, autoSave: true);
            }

            // 3. Destino del Usuario Ajeno (Londres)
            using (CambiarUsuario(_userAjenoId))
            {
                var destinoAjeno = new Destino(
                    _guidGenerator.Create(),
                    "Big Ben",
                    "Reino Unido",
                    "Londres",
                    8900000,
                    new Coordenadas(51.50m, -0.12m),
                    new Uri("http://dummy-image-3.com")
                );

                // Un ExternalId diferente
                destinoAjeno.SetExternalId("Q84");

                await _destinoRepository.InsertAsync(destinoAjeno, autoSave: true);
            }
        }

        [Fact]
        public async Task CrearEvento_Debe_Notificar_A_Usuarios_Con_Mismo_Destino_Geografico()
        {
            // Arrange
            await SeedDatosGeograficosAsync();

            // Obtenemos el ID del destino propiedad del creador para usarlo en el DTO
            var destinoDelCreador = (await _destinoRepository.GetListAsync(d => d.CreatorId == _userCreadorId)).First();

            var input = new CreateEventoDto
            {
                DestinoId = destinoDelCreador.Id, 
                Titulo = "Fiesta en Madrid",
                Descripcion = "Evento de prueba",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(2)
            };

            // Act
            
            using (CambiarUsuario(_userCreadorId))
            {
               
                await _destinoAppService.CrearEventoAsync(input);
            }

            // Assert
            // Verificamos la bandeja de notificaciones del Usuario Interesado
            var notificacionesInteresado = await _notificacionRepository.GetListAsync(n => n.UsuarioId == _userInteresadoId);

            notificacionesInteresado.ShouldNotBeEmpty();
            notificacionesInteresado.Count.ShouldBe(1);
            notificacionesInteresado.First().TituloDestino.ShouldBe("Torre Eiffel");
            notificacionesInteresado.First().CambioDetectado.ShouldContain("Nuevo evento en Torre Eiffel: Fiesta en Madrid.");
        }

        [Fact]
        public async Task CrearEvento_NO_Debe_Notificar_A_Usuarios_De_Otra_Ciudad()
        {
            // Arrange
            await SeedDatosGeograficosAsync();
            var destinoDelCreador = (await _destinoRepository.GetListAsync(d => d.CreatorId == _userCreadorId)).First();

            var input = new CreateEventoDto
            {
                DestinoId = destinoDelCreador.Id,
                Titulo = "Evento Exclusivo Madrid",
                Descripcion = "...",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now
            };

            // Act
            using (CambiarUsuario(_userCreadorId))
            {
                await _destinoAppService.CrearEventoAsync(input);
            }

            // Assert
            // El usuario que tiene "Barcelona" NO debería recibir nada
            var notificacionesAjeno = await _notificacionRepository.GetListAsync(n => n.UsuarioId == _userAjenoId);
            notificacionesAjeno.ShouldBeEmpty();
        }

        [Fact]
        public async Task UpdateDestino_Debe_Notificar_Cambio_Poblacion_A_Interesados()
        {
            // Arrange
            await SeedDatosGeograficosAsync();

            // Vamos a modificar el destino del creador (Madrid)
            var destinoDelCreador = (await _destinoRepository.GetListAsync(d => d.CreatorId == _userCreadorId)).First();

            var updateInput = new CreateUpdateDestinoDto
            {
                Nombre = destinoDelCreador.Nombre,
                Pais = destinoDelCreador.Pais,
                Ciudad = destinoDelCreador.Ciudad,
                Poblacion = 4000000,
                Latitud = destinoDelCreador.Ubicacion.Latitud,
                Longitud = destinoDelCreador.Ubicacion.Longitud,
                ImageUrl = destinoDelCreador.ImageUrl.ToString(),
                ExternalId = "Q90"
            };

            // Act
            using (CambiarUsuario(_userCreadorId))
            {
                await _destinoAppService.UpdateAsync(destinoDelCreador.Id, updateInput);
            }

            // Assert
            var notificacionesInteresado = await _notificacionRepository.GetListAsync(n => n.UsuarioId == _userInteresadoId);

            notificacionesInteresado.ShouldNotBeEmpty();
            notificacionesInteresado.First().CambioDetectado.ShouldContain("4000000");
        }
    }
}*/