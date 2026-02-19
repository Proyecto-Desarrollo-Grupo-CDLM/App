using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using MundiFavs.Destinos; // Para reusar DestinoDto al listar

namespace MundiFavs.Favoritos
{
    public interface IFavoritoAppService : IApplicationService
    {
        // 6.1 Agregar
        Task AddAsync(CreateFavoritoDto input);

        // 6.2 Eliminar
        Task RemoveAsync(Guid destinoId);

        // 6.3 Listar (Devolvemos los destinos completos)
        Task<List<DestinoDto>> GetListAsync();
    }
}