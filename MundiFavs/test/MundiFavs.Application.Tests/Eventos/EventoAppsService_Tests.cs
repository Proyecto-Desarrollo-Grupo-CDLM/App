using MundiFavs.Destinos;
using Shouldly; // Librería de aserciones muy legible
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace MundiFavs.Eventos
{

    public abstract class EventoAppService_Tests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IEventoAppService _eventoAppService;
        private readonly IRepository<Evento, Guid> _eventoRepository;
        private readonly IRepository<Destino, Guid> _destinoRepository;

        public EventoAppService_Tests()
        {
            // Obtenemos los servicios del contenedor de dependencias de prueba
            _eventoAppService = GetRequiredService<IEventoAppService>();
            _eventoRepository = GetRequiredService<IRepository<Evento, Guid>>();
            _destinoRepository = GetRequiredService<IRepository<Destino, Guid>>(); 
        }

        [Fact]
        public async Task GuardarEventoAsync_Deberia_Guardar_Evento_Nuevo()
        {
            // Arrange 
            var destinoPruebaId = Guid.NewGuid();

            var destinoFalso = new Destino(
                destinoPruebaId,
                "Madrid",
                "España",
                "Comunidad de Madrid",
                3000000,
                new Coordenadas(0, 0),
                new Uri("https://imagen.com/madrid.jpg")
            );
            destinoFalso.SetExternalId("TM-MADRID");

            // 👇 CAMBIO 1: Agregamos autoSave: true
            await _destinoRepository.InsertAsync(destinoFalso, autoSave: true);

            var input = new EventoDto
            {
                ExternalId = "TM-12345",
                DestinoId = destinoPruebaId,
                Nombre = "Concierto de Prueba",
                FechaInicio = DateTime.Now.AddDays(10),
                Url = "https://ticketmaster.com/evento-prueba",
                ImagenUrl = "https://image.com/prueba.jpg"
            };

            // Act 
            var result = await _eventoAppService.GuardarEventoAsync(input);

            // Assert 
            result.ShouldNotBeNull();
            result.Nombre.ShouldBe("Concierto de Prueba");

            // 👇 CAMBIO 2: Envolvemos la aserción en una nueva Unidad de Trabajo
            await WithUnitOfWorkAsync(async () =>
            {
                var eventoEnBd = await _eventoRepository.FirstOrDefaultAsync(e => e.ExternalId == "TM-12345");
                eventoEnBd.ShouldNotBeNull();
                eventoEnBd.DestinoId.ShouldBe(destinoPruebaId);
            });
        }

        [Fact]
        public async Task GuardarEventoAsync_Deberia_Lanzar_Excepcion_Si_Es_Duplicado_Para_El_Mismo_Destino()
        {
            // Arrange
            var destinoId = Guid.NewGuid();

            var destinoFalso = new Destino(
                destinoId,
                "Barcelona",
                "España",
                "Cataluña",
                1600000,
                new Coordenadas(0, 0),
                new Uri("https://imagen.com/barcelona.jpg")
            );
            destinoFalso.SetExternalId("TM-BARCELONA");

            // 👇 También agregamos autoSave: true por seguridad
            await _destinoRepository.InsertAsync(destinoFalso, autoSave: true);

            var inputOriginal = new EventoDto
            {
                ExternalId = "TM-DUPLICADO",
                DestinoId = destinoId,
                Nombre = "Evento Único",
                // 👇 CAMBIO: Agregamos los datos obligatorios que faltaban
                Url = "https://test.com/evento",
                ImagenUrl = "https://test.com/img.jpg",
                FechaInicio = DateTime.Now
            };

            // Guardamos el evento por primera vez
            await _eventoAppService.GuardarEventoAsync(inputOriginal);

            // Intentamos guardar exactamente el mismo evento para el mismo destino
            var inputDuplicado = new EventoDto
            {
                ExternalId = "TM-DUPLICADO",
                DestinoId = destinoId,
                Nombre = "Evento Copia",
                // 👇 CAMBIO: Agregamos los datos obligatorios que faltaban
                Url = "https://test.com/evento",
                ImagenUrl = "https://test.com/img.jpg",
                FechaInicio = DateTime.Now
            };

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _eventoAppService.GuardarEventoAsync(inputDuplicado);
            });
        }

        [Fact]
        public async Task BuscarEnTicketmasterAsync_Deberia_Traer_Resultados_De_API_Real()
        {
            

            // Arrange
            var ciudadReconocida = "London";

            // Act
            var resultados = await _eventoAppService.BuscarEnTicketmasterAsync(ciudadReconocida);

            // Assert

            resultados.ShouldNotBeNull();

            if (resultados.Count > 0)
            {
                resultados[0].ExternalId.ShouldNotBeNullOrEmpty();
                resultados[0].Nombre.ShouldNotBeNullOrEmpty();
            }
        }
    }
}