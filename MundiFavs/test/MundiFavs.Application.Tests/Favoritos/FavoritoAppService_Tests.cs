using MundiFavs.Destinos;
using MundiFavs.Favoritos;
using Shouldly; // Librería de aserciones muy cómoda que usa ABP
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace MundiFavs.Application.Tests.Favoritos
{
    // Heredamos de MundiFavsApplicationTestBase para tener todo el contexto de ABP
    public abstract class FavoritoAppService_Tests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
     where TStartupModule : IAbpModule
    {
        private readonly IFavoritoAppService _favoritoAppService;
        private readonly IRepository<Destino, Guid> _destinoRepository;

        public FavoritoAppService_Tests()
        {
            _favoritoAppService = GetRequiredService<IFavoritoAppService>();
            _destinoRepository = GetRequiredService<IRepository<Destino, Guid>>();
        }

        [Fact]
        public async Task Debe_Agregar_Favorito_Y_Listarlo()
        {
            // Arrange (Preparar)
            // Necesitamos un destino real en la BD para referenciarlo
            var destino = await CrearDestinoDePrueba();

            // Act (Actuar)
            await _favoritoAppService.AddAsync(new CreateFavoritoDto
            {
                DestinoId = destino.Id
            });

            // Assert (Verificar)
            var misFavoritos = await _favoritoAppService.GetListAsync();

            misFavoritos.ShouldNotBeNull();
            misFavoritos.Count.ShouldBe(1);
            misFavoritos.First().Nombre.ShouldBe("Paris Test");
        }

        [Fact]
        public async Task No_Debe_Duplicar_Favoritos()
        {
            // Arrange
            var destino = await CrearDestinoDePrueba();
            var input = new CreateFavoritoDto { DestinoId = destino.Id };

            // Agregamos el primer favorito de forma normal
            await _favoritoAppService.AddAsync(input);

            // Act & Assert
            // Verificamos que al intentar agregar el mismo destino, se lance la excepción de negocio
            var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
            {
                await _favoritoAppService.AddAsync(input);
            });

            // Validamos que el mensaje de la excepción sea exactamente el que esperamos
            exception.Message.ShouldBe("El Destino ya pertenece a tus Favoritos");

            // Opcional: Verificar que en la base de datos siga habiendo solo 1 registro
            var misFavoritos = await _favoritoAppService.GetListAsync();
            misFavoritos.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Debe_Eliminar_Favorito()
        {
            // Arrange
            var destino = await CrearDestinoDePrueba();
            await _favoritoAppService.AddAsync(new CreateFavoritoDto { DestinoId = destino.Id });

            // Act
            await _favoritoAppService.RemoveAsync(destino.Id);

            // Assert
            var misFavoritos = await _favoritoAppService.GetListAsync();
            misFavoritos.ShouldBeEmpty();
        }

        // Método auxiliar para crear datos de prueba
        private async Task<Destino> CrearDestinoDePrueba()
        {
            var coordenadas = new Coordenadas(48.8566m, 2.3522m);
            var url = new Uri("https://example.com/paris.jpg");
            var destino = new Destino(Guid.NewGuid(), "Paris Test", "Francia", "Una ciudad",10000,coordenadas,url);
            destino.SetExternalId("1234");
            return await _destinoRepository.InsertAsync(destino);
        }
    }
}