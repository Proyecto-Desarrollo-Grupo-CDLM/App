// En tu proyecto *.Application.Tests
using MundiFavs.Calificaciones;
using MundiFavs.Destinos;
using NSubstitute; // << Paquete de Mocks
using NSubstitute.ExceptionExtensions; // Para simular excepciones
using Shouldly;
using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities; // Para EntityNotFoundException
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using Xunit;



// TODO: Reemplaza estos 'usings' con los de tu proyecto
// using TuProyecto.Calificaciones;
// using TuProyecto.Calificaciones.Dtos;
// using TuProyecto.Destinos;

public class CalificacionAppService_UnitTests
{
    // --- Dependencias Mockeadas ---
    private readonly IRepository<Calificacion, Guid> _mockCalificacionRepo;
    private readonly IRepository<Destino, Guid> _mockDestinoRepo;
    private readonly ICurrentUser _mockCurrentUser;
    private readonly IGuidGenerator _mockGuidGenerator;
    private readonly IObjectMapper _mockObjectMapper;

    // --- Servicio bajo prueba ---
    private readonly CalificacionAppService _calificacionAppService;

    // --- Datos de prueba ---
    private static readonly Guid _testUserId = Guid.NewGuid();
    private static readonly Guid _testDestinoId = Guid.NewGuid();
    private static readonly Guid _testCalificacionId = Guid.NewGuid();

    public CalificacionAppService_UnitTests()
    {
        // 1. Crear todos los Mocks
        _mockCalificacionRepo = Substitute.For<IRepository<Calificacion, Guid>>();
        _mockDestinoRepo = Substitute.For<IRepository<Destino, Guid>>();
        _mockCurrentUser = Substitute.For<ICurrentUser>();
        _mockGuidGenerator = Substitute.For<IGuidGenerator>();
        _mockObjectMapper = Substitute.For<IObjectMapper>();

        

        // 2. Instanciar el AppService
        //    (Asumiendo un constructor que recibe todo esto)
        _calificacionAppService = new CalificacionAppService(
            _mockCalificacionRepo,  // 1. IRepository<Calificacion, Guid>
            _mockCurrentUser,       // 2. ICurrentUser
            _mockDestinoRepo,       // 3. IRepository<Destino, Guid>
            _mockGuidGenerator     // 4. IGuidGenerator
            );

        _calificacionAppService.ObjectMapperLazy = new Lazy<IObjectMapper>(() => _mockObjectMapper);
        // 3. Configuración común de Mocks

        // Simular que el GuidGenerator devuelve un ID predecible
        _mockGuidGenerator.Create().Returns(_testCalificacionId);

        // Simular que el usuario está autenticado (para la mayoría de pruebas)
        _mockCurrentUser.IsAuthenticated.Returns(true);
        _mockCurrentUser.Id.Returns(_testUserId);
       

        Coordenadas coord = new Coordenadas(488584, 22945);
        Uri imagenUri = new Uri("https://example.com/eiffel.jpg");
        // Simular que el Destino siempre existe (para la mayoría de pruebas)
        var mockDestino = new Destino(_testDestinoId, "Torre Eiffel", "París", "Francia", 30000000, coord,imagenUri );
        _mockDestinoRepo.GetAsync(_testDestinoId).Returns(mockDestino);

        // Simular que el Mapeo funciona
        _mockObjectMapper.Map<Calificacion, CalificacionDto>(Arg.Any<Calificacion>())
            .Returns(new CalificacionDto()); // Devolver un DTO vacío genérico
    }

    // --- INICIO DE PRUEBAS ---

    [Fact]
    
    public async Task CreateAsync_Debe_Guardar_Calificacion_Valida()
    {
        // ARRANGE
 
        var input = new CreateUpdateCalificacionDto
        {
            DestinoId = _testDestinoId,
            Estrellas = 5,
            Comentario = "¡Genial!"
        };

        var mockDestino = new Destino(_testDestinoId, "Destino Falso", "Ciudad Falsa", "País Falso", 30000000, new Coordenadas(1, 1), new Uri("http://a.com"));
        _mockDestinoRepo.GetAsync(_testDestinoId).Returns(mockDestino);

        // Simular que no hay duplicados
         _mockCalificacionRepo.FirstOrDefaultAsync(Arg.Any<Expression<Func<Calificacion, bool>>>())
              .Returns((Calificacion)null);

        // ACT
        await _calificacionAppService.CreateAsync(input);

        // ASSERT
        // Verificamos que se llamó a InsertAsync con los datos correctos
        await _mockCalificacionRepo.Received(1).InsertAsync(
            Arg.Is<Calificacion>(c =>
                c.Id == _testCalificacionId && // Guid generado
                c.UserId == _testUserId &&       // Usuario actual
                c.Destino.Id == _testDestinoId && // Destino obtenido
                c.Estrellas == 5 &&
                c.Comentario == "¡Genial!"),
            true 
        );
    }

    [Fact]
    public async Task CreateAsync_Debe_Guardar_Con_Comentario_Nulo()
    {
        // ARRANGE
        var input = new CreateUpdateCalificacionDto
        {
            DestinoId = _testDestinoId,
            Estrellas = 3,
            Comentario = null // Comentario nulo
        };

        var mockDestino = new Destino(_testDestinoId, "Destino Falso", "Ciudad Falsa", "País Falso", 30000000, new Coordenadas(1, 1), new Uri("http://a.com"));
        _mockDestinoRepo.GetAsync(_testDestinoId).Returns(mockDestino);

        // Simular que no hay duplicados
            _mockCalificacionRepo.FirstOrDefaultAsync(Arg.Any<Expression<Func<Calificacion, bool>>>())
            .Returns((Calificacion)null);

        // ACT
        await _calificacionAppService.CreateAsync(input);

        // ASSERT
        // Verificamos que el comentario se pasó como nulo
        await _mockCalificacionRepo.Received(1).InsertAsync(
            Arg.Is<Calificacion>(c =>
                c.Estrellas == 3 &&
                c.Destino.Id == _testDestinoId &&
                c.Comentario == null), // El comentario es nulo
            true
        );
    }

    [Fact]

    public async Task CrearAsync_Debe_Lanzar_Excepcion_Si_Calificacion_Es_Duplicada()
    {
        

        // ARRANGE
        var input = new CreateUpdateCalificacionDto
        {
            DestinoId = _testDestinoId,
            Estrellas = 1
        };

        var mockDestino = new Destino(_testDestinoId, "Destino Falso", "Ciudad Falsa", "País Falso", 30000000, new Coordenadas(1, 1), new Uri("http://a.com"));
        _mockDestinoRepo.GetAsync(_testDestinoId).Returns(mockDestino);

        // 2. Simular que YA EXISTE una calificación
        var calificacionExistente = new Calificacion(
            Guid.NewGuid(), 5, "Ya existe", mockDestino, _testUserId
        );

       
        _mockCalificacionRepo.FirstOrDefaultAsync(Arg.Any<Expression<Func<Calificacion, bool>>>())
            .Returns(calificacionExistente);

        // ACT & ASSERT
        // Ahora esta prueba SÍ funcionará, porque el AppService tiene la lógica
        var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
        {
            await _calificacionAppService.CreateAsync(input);
        });

        exception.Message.ShouldBe("Ya has calificado este destino.");

        // Verificamos que NUNCA se llamó a InsertAsync
        await _mockCalificacionRepo.DidNotReceive().InsertAsync(Arg.Any<Calificacion>(), Arg.Any<bool>());
    }



    [Fact]  
    public async Task CrearAsync_Debe_Lanzar_Excepcion_Si_Destino_No_Existe()
    {
        // ARRANGE
        var input = new CreateUpdateCalificacionDto
        {
            DestinoId = _testDestinoId,
            Estrellas = 5
        };

        // Simular que GetAsync falla y no encuentra el destino
        _mockDestinoRepo.GetAsync(_testDestinoId)
            .Throws(new EntityNotFoundException(typeof(Destino), _testDestinoId));

        // ACT & ASSERT
        // Verificamos que la excepción EntityNotFoundException se propaga
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await _calificacionAppService.CreateAsync(input);
        });

        // Verificamos que NUNCA se llamó a InsertAsync
        await _mockCalificacionRepo.DidNotReceive().InsertAsync(Arg.Any<Calificacion>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CrearAsync_Debe_Lanzar_Excepcion_Si_Usuario_No_Esta_Autenticado()
    {
        // ARRANGE
        var input = new CreateUpdateCalificacionDto
        {
            DestinoId = _testDestinoId,
            Estrellas = 5
        };

        // Simular que el usuario NO está autenticado
        _mockCurrentUser.IsAuthenticated.Returns(false);
        _mockCurrentUser.Id.Returns((Guid?)null);

        // ACT & ASSERT
        // El código `_currentUser.Id.Value` lanzará una excepción.
        // O, si lo validas, debería lanzar AbpAuthorizationException.
        // Asumiremos que falla al intentar acceder a .Value
        await Should.ThrowAsync<Exception>(async () =>
        {
            await _calificacionAppService.CreateAsync(input);
        });

        // Verificamos que NUNCA se llamó a InsertAsync
        await _mockCalificacionRepo.DidNotReceive().InsertAsync(Arg.Any<Calificacion>(), Arg.Any<bool>());
    }
}