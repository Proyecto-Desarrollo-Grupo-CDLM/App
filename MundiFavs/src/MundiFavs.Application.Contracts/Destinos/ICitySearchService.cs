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
        Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request);

    }
}
    