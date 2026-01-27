using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MundiFavs.Calificaciones
{
    public interface ICalificacionAppService : ICrudAppService<
        CalificacionDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateCalificacionDto>
    {
        // 👇 AGREGA ESTE MÉTODO NUEVO
        Task<CalificacionDto?> GetMyCalificacionAsync(Guid destinoId);
    }
}