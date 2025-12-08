using Microsoft.EntityFrameworkCore;
using MundiFavs.Calificaciones;
using MundiFavs.Destinos;
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
        }

       
        
        [Fact]
        public async Task NotCrearCalificacionWhenNotLogin()
        {

            var destinoId = Guid.NewGuid();
            var coordenadas = new Coordenadas(488566, 23522);
            var url = new Uri("https://example.com/paris.jpg");

            await WithUnitOfWorkAsync(async () =>
            {
                await _destinoRepository.InsertAsync(new Destino(destinoId, "Paris","Francia", "Paris", 2148000, coordenadas,url));
            });

            var input = new CreateUpdateCalificacionDto

            {
                DestinoId = destinoId,
                Estrellas = 5
            };

            using (_currentPrincipalAccessor.Change(new ClaimsPrincipal(new ClaimsIdentity())))
            {

                await Should.ThrowAsync<AbpAuthorizationException>(async () =>
                {
                    await _calificacionAppService.CreateAsync(input);
                });

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
            await WithUnitOfWorkAsync(async () =>
            {
                await _destinoRepository.InsertAsync(new Destino(destinoId, "Paris", "Francia", "Paris", 2148000, coordenadas, url));
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
                    Estrellas = 5,
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
                new Calificacion(guid, 0, "Inválido", destinoPrueba, guid);
            });


            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                new Calificacion(guid, 6, "Inválido",destinoPrueba, guid);
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


            var calificacionMin = new Calificacion(guid, 1, "Válido",destinoPrueba,guid);
            calificacionMin.Estrellas.ShouldBe(1);


            var calificacionMax = new Calificacion(guid, 5, "Válido",destinoPrueba,guid);
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
           
            await WithUnitOfWorkAsync(async () =>
            {
                await _destinoRepository.InsertAsync(new Destino(destinoId, "Paris", "Francia", "Paris", 2148000, coordenadas, url));
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
                    Estrellas = 4,
                    Comentario = null
                };


                await _calificacionAppService.CreateAsync(input);


                await WithUnitOfWorkAsync(async () =>
                {
                    var calificacion = await _calificacionRepository.FirstOrDefaultAsync(c => c.DestinoId == destinoId);

                    calificacion.ShouldNotBeNull();
                    calificacion.Estrellas.ShouldBe(4);
                    calificacion.Comentario.ShouldBeNull();
                    calificacion.UserId.ShouldBe(userId);
                });
            }
        }
        [Fact]
        public async Task Should_Throw_Exception_When_Rating_Same_Destino_Twice()
        {

            var userId = Guid.NewGuid();
            var username = "testuser-duplicate";
            await WithUnitOfWorkAsync(async () => { new IdentityUser(userId, username, "testuser@example.com"); });

            var destinoId = Guid.NewGuid();
            var coordenadas = new Coordenadas(488566, 23522);
            var url = new Uri("https://example.com/paris.jpg");
            await WithUnitOfWorkAsync(async () => { new Destino(destinoId, "Paris", "Francia", "Paris", 2148000, coordenadas, url); });

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
                    Estrellas = 5,
                    Comentario = "Primera vez"
                };


                await _calificacionAppService.CreateAsync(input);


                var inputDuplicado = new CreateUpdateCalificacionDto
                {
                    DestinoId = destinoId,
                    Estrellas = 1,
                    Comentario = "Segunda vez"
                };


                var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
                {
                    await _calificacionAppService.CreateAsync(inputDuplicado);
                });


                exception.Message.ShouldBe("Ya has calificado este destino.");
            }
        }
    }
}