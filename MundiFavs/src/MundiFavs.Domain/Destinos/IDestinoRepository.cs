using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MundiFavs.Destinos
{
    public interface IDestinoRepository : IRepository<Destino, Guid>
    {
        // Obtiene los destinos más guardados, agrupados por nombre/país
        Task<List<Destino>> GetPopularAsync(int maxCount = 10);
    }
}