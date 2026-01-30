using System;
﻿using Microsoft.AspNetCore.Authorization;
using MundiFavs.Destinos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        [Authorize] // Asegura que solo usuarios autenticados puedan usar estos métodos
        Task<CalificacionDto> UpdateCalificacionAsync(Guid id, UpdateCalificacionDto input);

    }
}