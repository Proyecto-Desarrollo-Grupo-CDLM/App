using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using MundiFavs.CitySearch;

namespace MundiFavs.Destinos;
public class DestinoAppService :
    CrudAppService<
        Destino, //The  Destino entity
        DestinoDto, //Used to show Destino
        Guid, //Primary key of the destino entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateDestinoDto>, //Used to create/update a destino
    IDestinoAppService //implement the IDestinoAppService
{
    private readonly ICitySearchService _citySearchService;

    public DestinoAppService(IRepository<Destino, Guid> repository, ICitySearchService citySearchService)
        : base(repository)
    {
        _citySearchService = citySearchService;
    }

   public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
    {
        return await _citySearchService.SearchCitiesAsync(request);
    }
  
}