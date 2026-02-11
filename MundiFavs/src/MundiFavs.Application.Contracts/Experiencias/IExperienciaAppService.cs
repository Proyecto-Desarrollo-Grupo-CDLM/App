using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MundiFavs.Experiencias
{
    public interface IExperienciaAppService : IApplicationService
    {
        Task<ExperienciaDto> CreateAsync(CreateUpdateExperienciaDto input);
        Task<ExperienciaDto> UpdateAsync(Guid id, CreateUpdateExperienciaDto input);
        Task DeleteAsync(Guid id);
        Task<PagedResultDto<ExperienciaDto>> GetListAsync(GetExperienciasInput input);
    }
}