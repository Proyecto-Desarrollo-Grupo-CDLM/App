using Microsoft.EntityFrameworkCore;
using MundiFavs.Calificaciones;
using MundiFavs.Destinos;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace MundiFavs.Tests.Calificaciones
{
    public abstract class CalificacionAppService_IntegrationTests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
         where TStartupModule : IAbpModule

    {
        private readonly ICalificacionAppService _calificacionAppService;
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly IRepository<Calificacion, Guid> _calificacionRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
        private readonly IdentityUserManager _identityUserManager;

        public CalificacionAppService_IntegrationTests()
        {
            _calificacionAppService = GetRequiredService<ICalificacionAppService>();
            _destinoRepository = GetRequiredService<IRepository<Destino, Guid>>();
            _calificacionRepository = GetRequiredService<IRepository<Calificacion, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _identityUserManager = GetRequiredService<IdentityUserManager>();
            _currentUser= Substitute.For<ICurrentUser>();
        }

       
        
        [Fact]
        public async Task NotCrearCalificacionWhenNotLogin()
        {
            // 1. Arrange
            var destinoId = Guid.NewGuid();
            var coordenadas = new Coordenadas(488566, 23522);
            var url = new Uri("https://example.com/paris.jpg");

            // Preparamos el destino correctamente
            await WithUnitOfWorkAsync(async () =>
            {
                var destino = new Destino(destinoId, "Paris", "Francia", "Paris", 2148000, coordenadas, url);
                destino.SetExternalId("TEST_AUTH_CHECK");
                await _destinoRepository.InsertAsync(destino, autoSave: true);
            });

            var input = new CreateUpdateCalificacionDto
            {
                DestinoId = destinoId,
                Puntuacion = 5,
                Comentario = "Intento hacking"
            };

            // 2. Act & Assert
            // Simulamos un usuario ANÓNIMO (sin claims, sin ID)
            using (_currentPrincipalAccessor.Change(new ClaimsPrincipal(new ClaimsIdentity())))
            {
                // IMPORTANTE: Si esto sigue tirando DbUpdateException, es 100% seguro 
                // que te falta el [Authorize] en el AppService.
                await Should.ThrowAsync<AbpAuthorizationException>(async () =>
                {
                    await _calificacionAppService.CreateAsync(input);
                });
            }
        }

        [Fact]
        public async Task ShouldCreateCalificacionWhenLoggedIn()
        {
            var destinoId = Guid.NewGuid();
            var coordenadas = new Coordenadas(488566, 23522);
            var url = new Uri("https://example.com/paris.jpg");

            // CORRECCIÓN
            await WithUnitOfWorkAsync(async () =>
            {
                var destino = new Destino(destinoId, "Paris", "Francia", "Paris", 2148000, coordenadas, url);
                destino.SetExternalId("TEST_LOGGED_USER"); // <--- FALTABA ESTO
                await _destinoRepository.InsertAsync(destino, autoSave: true);
            });

            var userId = Guid.NewGuid();
            var username = "testuser";

            await WithUnitOfWorkAsync(async () =>
            {
                var user = new IdentityUser(userId, username, "testuser@example.com");
                var identityResult = await _identityUserManager.CreateAsync(user, "TestPassword123!");
                identityResult.Succeeded.ShouldBeTrue();
            });

            var claimsprincipal = new ClaimsPrincipal(
                     new ClaimsIdentity(
                         new Claim[]
                         {
                 new Claim(AbpClaimTypes.UserName, username),
                 new Claim(AbpClaimTypes.UserId,userId.ToString()),
                         }));

            using (_currentPrincipalAccessor.Change(claimsprincipal))
            {
                var input = new CreateUpdateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 5,
                    Comentario = "Prueba de integración"
                };
                await _calificacionAppService.CreateAsync(input);

                await WithUnitOfWorkAsync(async () =>
                {
                    var calificacion = await _calificacionRepository.FirstOrDefaultAsync(c => c.DestinoId == destinoId);
                    calificacion.ShouldNotBeNull();
                    calificacion.Estrellas.ShouldBe(5);
                    calificacion.Comentario.ShouldBe("Prueba de integración");
                    calificacion.UserId.ShouldBe(userId);
                });
            }
        }
        [Fact]
        public void Should_Throw_Exception_When_Puntaje_Is_Out_Of_Range()
        {
            var guid = Guid.NewGuid();
            var destinoId = Guid.NewGuid();
            var coordenadas = new Coordenadas(488566, 23522);
            var url = new Uri("https://example.com/paris.jpg");
            var destinoPrueba = new Destino(destinoId, "Paris", "Francia", "Paris", 2148000, coordenadas, url);

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                new Calificacion(guid, 0, "Inválido", destinoPrueba.Id, guid);
            });


            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                new Calificacion(guid, 6, "Inválido",destinoPrueba.Id, guid);
            });
        }
        [Fact]
        public void Should_Create_Successfully_With_Valid_Puntaje()
        {
            var guid = Guid.NewGuid();
            var destinoId = Guid.NewGuid();
            var coordenadas = new Coordenadas(488566, 23522);
            var url = new Uri("https://example.com/paris.jpg");
            var destinoPrueba = new Destino(destinoId, "Paris", "Francia", "Paris", 2148000, coordenadas, url);


            var calificacionMin = new Calificacion(guid, 1, "Válido",destinoPrueba.Id,guid);
            calificacionMin.Estrellas.ShouldBe(1);


            var calificacionMax = new Calificacion(guid, 5, "Válido",destinoPrueba.Id,guid);
            calificacionMax.Estrellas.ShouldBe(5);
        }
        [Fact]
        public async Task Should_Create_Rating_Without_Comentario()
        {
            var userId = Guid.NewGuid();
            var username = "testuser-nocomment";
            await WithUnitOfWorkAsync(async () =>
            {
                (await _identityUserManager.CreateAsync(new IdentityUser(userId, username, "test@example.com"), "TestPassword123!")).Succeeded.ShouldBeTrue();
            });

            var destinoId = Guid.NewGuid();
            var coordenadas = new Coordenadas(488566, 23522);
            var url = new Uri("https://example.com/paris.jpg");

            // CORRECCIÓN: Separamos la creación para poder llamar a SetExternalId
            await WithUnitOfWorkAsync(async () =>
            {
                var destino = new Destino(destinoId, "Paris", "Francia", "Paris", 2148000, coordenadas, url);
                destino.SetExternalId("TEST_Q99"); // <--- ESTO FALTABA
                await _destinoRepository.InsertAsync(destino, autoSave: true);
            });

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                        new Claim[]
                        {
                new Claim(AbpClaimTypes.UserName, username),
                new Claim(AbpClaimTypes.UserId,userId.ToString()),
                        }));

            using (_currentPrincipalAccessor.Change(claimsPrincipal))
            {
                var input = new CreateUpdateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 4,
                    Comentario = null
                };

                await _calificacionAppService.CreateAsync(input);

                await WithUnitOfWorkAsync(async () =>
                {
                    var calificacion = await _calificacionRepository.FirstOrDefaultAsync(c => c.DestinoId == destinoId);

                    calificacion.ShouldNotBeNull();
                    calificacion.Estrellas.ShouldBe(4);
                    calificacion.Comentario.ShouldBeNull(); // O string.Empty según tu lógica
                    calificacion.UserId.ShouldBe(userId);
                });
            }
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Rating_Same_Destino_Twice()
        {
            // 1. Crear Usuario
            var userId = Guid.NewGuid();
            var username = "testuser-duplicate";
            await WithUnitOfWorkAsync(async () => {
                await _identityUserManager.CreateAsync(new IdentityUser(userId, username, "testuser@example.com"));
            });

            // 2. Crear Destino
            var destinoId = Guid.NewGuid();
            var coordenadas = new Coordenadas(488566, 23522);
            var url = new Uri("https://example.com/paris.jpg");

            await WithUnitOfWorkAsync(async () => {
                var destino = new Destino(destinoId, "Paris", "Francia", "Paris", 2148000, coordenadas, url);
                destino.SetExternalId("TEST_DUPLICATE_CHECK");
                await _destinoRepository.InsertAsync(destino, autoSave: true);
            });

            // 3. Preparar contexto de seguridad (Login)
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
            new Claim(AbpClaimTypes.UserName, username),
            new Claim(AbpClaimTypes.UserId, userId.ToString()),
                }));

            using (_currentPrincipalAccessor.Change(claimsPrincipal))
            {
                var input1 = new CreateUpdateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 5,
                    Comentario = "Primera vez"
                };

                // --- CAMBIO IMPORTANTE ---
                // Ejecutamos la primera inserción dentro de un UnitOfWork propio para asegurar que se guarde en BD.
                await WithUnitOfWorkAsync(async () =>
                {
                    await _calificacionAppService.CreateAsync(input1);
                });
                // -------------------------

                var inputDuplicado = new CreateUpdateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 1,
                    Comentario = "Segunda vez (intento)"
                };

                // Ahora intentamos insertar la segunda. Como la primera ya está "commiteada" en la BD,
                // la validación del servicio debería encontrarla y lanzar la excepción.
                var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
                {
                    await _calificacionAppService.CreateAsync(inputDuplicado);
                });

                exception.Message.ShouldBe("Ya has calificado este destino.");
            }
        }
    }
}