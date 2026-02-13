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
            misFavoritos.First().Nombre.ShouldBe("Paris");
        }

        [Fact]
        public async Task No_Debe_Duplicar_Favoritos()
        {
            // Arrange
            var destino = await CrearDestinoDePrueba();
            var input = new CreateFavoritoDto { DestinoId = destino.Id };

            // Act - Primera inserción
            await _favoritoAppService.AddAsync(input);

            // Act & Assert - Segunda inserción 
            // 1. ASIGNAMOS el error a la variable 'excepcion'
            var excepcion = await Should.ThrowAsync<UserFriendlyException>(async () =>
            {
                await _favoritoAppService.AddAsync(input);
            });

            excepcion.Message.ShouldContain("El Destino ya pertenece a tus Favoritos");

            // Assert Final
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

            var coordenadas = new Coordenadas(48.7566m, 2.3522m);

            var url = new Uri("https://example.com/paris.jpg");
            var destino = new Destino(Guid.NewGuid(), "Paris Test", "Francia", "Una ciudad",10000,coordenadas,url);
            destino.SetExternalId("1234");
            return await _destinoRepository.InsertAsync(destino);

        }
    }
}