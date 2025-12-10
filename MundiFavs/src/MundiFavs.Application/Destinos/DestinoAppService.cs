using MundiFavs.CitySearch; 
using System;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using static System.Net.WebRequestMethods;

namespace MundiFavs.Destinos;

public class DestinoAppService :
    CrudAppService<
        Destino,
        DestinoDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateDestinoDto>,
    IDestinoAppService
{

    private readonly ICitySearchService _citySearchService;

    public DestinoAppService(
        IRepository<Destino, Guid> repository,
        ICitySearchService citySearchService) 
        : base(repository)
    {
        _citySearchService = citySearchService;
    }

    // ----------------------------------------------------------------------
    //          IMPLEMENTACIÓN DEL REQUERIMIENTO 3.5
    // ----------------------------------------------------------------------
    public async Task<DestinoDto> SaveFromCitySearchAsync(CiudadDto ciudadSummary)
    {
        // 1. Obtener el detalle completo de la API externa (Operación 3.3)
        var cityDetailRequest = new CityDetailRequestDto { CityId = ciudadSummary.Id };
        CityDetailDto cityDetail;

        try
        {
            
            cityDetail = await _citySearchService.GetCityDetailById(cityDetailRequest);
        }
        catch (EntityNotFoundException)
        {
            throw new UserFriendlyException("El detalle de la ciudad seleccionada no se encontró en la API externa. Intente buscar de nuevo.");
        }

        // 2. Mapear CityDetailDto a CreateUpdateDestinoDto (DTO de CRUD)
        var createDto = new CreateUpdateDestinoDto
        {
            Nombre = cityDetail.Region,// mapeamos region en campo nombre porque no tenemos nombre del destino especifico
            Pais = cityDetail.Pais,
            Ciudad = cityDetail.NombreCiudad, 
            Poblacion = (int)cityDetail.Poblacion,
            Latitud = cityDetail.Latitud,
            Longitud = cityDetail.Longitud,
            ImageUrl = ""
        };

        // 3. Llamar al método base CreateAsync del CrudAppService (Internamente usa el repositorio)
        // Sobrescribimos la conversión de Uri para asegurar la validación.

        // Creamos la Entidad Destino
        var destino = new Destino(
             id: GuidGenerator.Create(),
             nombre: createDto.Nombre,
             pais: createDto.Pais,
             ciudad: createDto.Ciudad,
             poblacion: createDto.Poblacion,
             ubicacion: new Coordenadas(createDto.Latitud, createDto.Longitud),
             imageUrl: new Uri("https://example.com")// El DTO no tiene URL de foto, la dejamos nula
        );

        // Insertamos la Entidad
        var savedEntity = await Repository.InsertAsync(destino, autoSave: true);

        // Mapeamos y devolvemos el DTO de salida
        return ObjectMapper.Map<Destino, DestinoDto>(savedEntity);
    }
}