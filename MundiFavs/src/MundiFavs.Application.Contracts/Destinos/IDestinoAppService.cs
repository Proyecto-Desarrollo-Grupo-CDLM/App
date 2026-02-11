using MundiFavs.CitySearch;
using System;
using System.Collections.Generic; // Necesario para List<>
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
// using MundiFavs.CitySearch; // (Déjalo solo si tus DTOs de búsqueda están en otro namespace)

namespace MundiFavs.Destinos
{
    public interface IDestinoAppService :
        ICrudAppService<
            DestinoDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateDestinoDto>
    {
        // Método de búsqueda de ciudades
        Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request);

        // Método para destinos populares (Tu cambio anterior)
        Task<List<DestinoDto>> GetPopularDestinationsAsync(int maxCount = 10);

        // --- NUEVO MÉTODO (Paso 1) ---
        // Este trae "Mis Destinos" paginados y filtrados por usuario
        Task<PagedResultDto<DestinoDto>> GetMyDestinationsAsync(PagedAndSortedResultRequestDto input);

        Task<DestinoComentariosDto> GetComentariosConPromedioAsync(string externalCityId);
    }
}