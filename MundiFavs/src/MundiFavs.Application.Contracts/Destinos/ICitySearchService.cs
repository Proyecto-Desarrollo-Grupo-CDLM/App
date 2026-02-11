using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MundiFavs.CitySearch
{
    public interface ICitySearchService : IApplicationService
    {
        // Metodo para la operación 3.1: Buscar ciudades por nombre.
        Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request);

        // Metodo para la operaíón 3.3: Obtener detalles de una ciudad.
        Task<CityDetailDto> GetCityDetailById(CityDetailRequestDto input);

    }
}
    