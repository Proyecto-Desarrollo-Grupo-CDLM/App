using System;
using System.Collections.Generic; // Necesario para List<>
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using MundiFavs.CitySearch;

namespace MundiFavs.Destinos
{
    public interface IDestinoAppService :
        ICrudAppService<
            DestinoDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateDestinoDto>
    {
        Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request);

        // CAMBIO 1: Cambiamos el retorno a List<DestinoDto> para que coincida con tu frontend (DestinoDto[])
        // CAMBIO 2: Agregamos el parámetro (int maxCount) para que coincida con la llamada (10)
        Task<List<DestinoDto>> GetPopularDestinationsAsync(int maxCount = 10);
    }
}