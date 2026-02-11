using MundiFavs.Destinos;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace MundiFavs.Experiencias
{
    public class ExperienciaAppService_Tests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IExperienciaAppService _experienciaAppService;
        private readonly IRepository<Experiencia, Guid> _experienciaRepository;

        
        private Guid _testDestinoId;

        public ExperienciaAppService_Tests()
        {
            _experienciaAppService = GetRequiredService<IExperienciaAppService>();
            _experienciaRepository = GetRequiredService<IRepository<Experiencia, Guid>>();

            
            _testDestinoId = Guid.NewGuid();
        }

        [Fact]
        public async Task Should_Create_Experiencia_Successfully()
        {
            // Arrange
            var input = new CreateUpdateExperienciaDto
            {
                DestinoId = _testDestinoId,
                Comentario = "Una experiencia increíble en este lugar.",
                Valoracion = Valoracion.Positiva, 
                Etiquetas = "turismo,comida,sol",
                FechaExperiencia = DateTime.Now.AddDays(-5)
            };

            // Act
            var result = await _experienciaAppService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.Comentario.ShouldBe(input.Comentario);
            result.Valoracion.ShouldBe(Valoracion.Positiva);
            result.Etiquetas.ShouldBe("turismo,comida,sol");
        }

        [Fact]
        public async Task Should_Get_List_Filtered_By_Destino()
        {
            // Arrange: Creamos 2 experiencias, una para el destino A y otra para el B
            var destinoA = Guid.NewGuid();
            var destinoB = Guid.NewGuid();

            await _experienciaAppService.CreateAsync(new CreateUpdateExperienciaDto
            {
                DestinoId = destinoA,
                Comentario = "Destino A es genial",
                Valoracion = Valoracion.Positiva,
                Etiquetas = "A",
                FechaExperiencia = DateTime.Now
            });

            await _experienciaAppService.CreateAsync(new CreateUpdateExperienciaDto
            {
                DestinoId = destinoB,
                Comentario = "Destino B es regular",
                Valoracion = Valoracion.Neutral,
                Etiquetas = "B",
                FechaExperiencia = DateTime.Now
            });

            // Act: Buscamos solo las del destino A
            var result = await _experienciaAppService.GetListAsync(new GetExperienciasInput
            {
                DestinoId = destinoA
            });

            // Assert
            result.TotalCount.ShouldBe(1);
            result.Items.First().Comentario.ShouldBe("Destino A es genial");
        }

        [Fact]
        public async Task Should_Update_Own_Experiencia()
        {
            // Arrange: Crear una experiencia con el usuario actual
            var created = await _experienciaAppService.CreateAsync(new CreateUpdateExperienciaDto
            {
                DestinoId = _testDestinoId,
                Comentario = "Original",
                Valoracion = Valoracion.Neutral,
                Etiquetas = "original",
                FechaExperiencia = DateTime.Now
            });

            // Act: Actualizarla
            var updateInput = new CreateUpdateExperienciaDto
            {
                DestinoId = _testDestinoId,
                Comentario = "Actualizado",
                Valoracion = Valoracion.Negativa, // Cambio de opinión
                Etiquetas = "editado",
                FechaExperiencia = DateTime.Now
            };

            var updated = await _experienciaAppService.UpdateAsync(created.Id, updateInput);

            // Assert
            updated.Comentario.ShouldBe("Actualizado");
            updated.Valoracion.ShouldBe(Valoracion.Negativa);

            // Verificar persistencia en DB
            var dbEntity = await _experienciaRepository.GetAsync(created.Id);
            dbEntity.Comentario.ShouldBe("Actualizado");
        }

        [Fact]
        public async Task Should_Not_Update_Others_Experiencia()
        {
            // Arrange: Insertar MANUALMENTE una experiencia en la DB simulando OTRO usuario
            var otherUserId = Guid.NewGuid();
            var experienciaAjena = new Experiencia(
                Guid.NewGuid(),
                otherUserId, // Usuario dueño diferente al actual
                _testDestinoId,
                "Experiencia de otro",
                Valoracion.Positiva,
                "otro",
                DateTime.Now
            );

            await _experienciaRepository.InsertAsync(experienciaAjena);

            // Act & Assert
            // Intentamos actualizar usando el _experienciaAppService (que corre como el usuario de prueba por defecto)
            var exception = await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await _experienciaAppService.UpdateAsync(experienciaAjena.Id, new CreateUpdateExperienciaDto
                {
                    DestinoId = _testDestinoId,
                    Comentario = "Hackeando comentario",
                    Valoracion = Valoracion.Negativa,
                    Etiquetas = "hacked",
                    FechaExperiencia = DateTime.Now
                });
            });

            exception.Message.ShouldContain("No puedes editar una experiencia que no es tuya");
        }

        [Fact]
        public async Task Should_Filter_By_Valoracion()
        {
            // Arrange: Crear experiencias con distintas valoraciones
            await _experienciaAppService.CreateAsync(new CreateUpdateExperienciaDto
            {
                DestinoId = _testDestinoId,
                Comentario = "Bien",
                Valoracion = Valoracion.Positiva,
                Etiquetas = "1",
                FechaExperiencia = DateTime.Now
            });
            await _experienciaAppService.CreateAsync(new CreateUpdateExperienciaDto
            {
                DestinoId = _testDestinoId,
                Comentario = "Mal",
                Valoracion = Valoracion.Negativa,
                Etiquetas = "2",
                FechaExperiencia = DateTime.Now
            });

            // Act: Filtrar solo Negativas
            var result = await _experienciaAppService.GetListAsync(new GetExperienciasInput
            {
                Valoracion = Valoracion.Negativa
            });

            // Assert
            result.Items.ShouldContain(x => x.Comentario == "Mal");
            result.Items.ShouldNotContain(x => x.Comentario == "Bien");
        }

        [Fact]
        public async Task Should_Delete_Own_Experiencia()
        {
            // Arrange: Crear una experiencia propia
            var createdExperience = await _experienciaAppService.CreateAsync(new CreateUpdateExperienciaDto
            {
                DestinoId = _testDestinoId,
                Comentario = "Comentario a eliminar",
                Valoracion = Valoracion.Neutral,
                Etiquetas = "borrar",
                FechaExperiencia = DateTime.Now
            });

            // Act: Eliminarla
            await _experienciaAppService.DeleteAsync(createdExperience.Id);

            // Assert: Verificar que ya no existe en base de datos
            var deletedExperience = await _experienciaRepository.FindAsync(createdExperience.Id);
            deletedExperience.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Not_Delete_Others_Experiencia()
        {
            // Arrange: Insertar MANUALMENTE una experiencia de OTRO usuario
            var otherUserId = Guid.NewGuid();
            var experienciaAjena = new Experiencia(
                Guid.NewGuid(),
                otherUserId, // Dueño diferente al actual
                _testDestinoId,
                "Experiencia que no es mía",
                Valoracion.Positiva,
                "seguridad",
                DateTime.Now
            );

            await _experienciaRepository.InsertAsync(experienciaAjena);

            // Act & Assert: Intentar eliminarla con el usuario actual debe fallar
            var exception = await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await _experienciaAppService.DeleteAsync(experienciaAjena.Id);
            });

            exception.Message.ShouldContain("No puedes eliminar una experiencia que no es tuya");

            // Assert Adicional: Verificar que la experiencia SIGUE existiendo en la DB
            var existingExperience = await _experienciaRepository.FindAsync(experienciaAjena.Id);
            existingExperience.ShouldNotBeNull();
        }
    }
}