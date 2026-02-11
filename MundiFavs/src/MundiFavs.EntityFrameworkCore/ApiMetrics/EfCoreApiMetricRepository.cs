using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MundiFavs.Domain.ApiMetrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MundiFavs.EntityFrameworkCore.ApiMetrics
{
    public class EfCoreApiMetricRepository : EfCoreRepository<MundiFavsDbContext, ApiMetric, Guid>, IApiMetricRepository
    {
        public EfCoreApiMetricRepository(IDbContextProvider<MundiFavsDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        public async Task<List<ApiMetric>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(m => m.RequestDateTime >= startDate && m.RequestDateTime <= endDate)
                .OrderByDescending(m => m.RequestDateTime)
                .ToListAsync();
        }

        public async Task<List<ApiMetric>> GetByEndpointAsync(string endpoint, int maxResults = 100)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(m => m.Endpoint == endpoint)
                .OrderByDescending(m => m.RequestDateTime)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<List<ApiMetric>> GetErrorMetricsAsync(DateTime startDate, DateTime endDate)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(m => !m.IsSuccess && m.RequestDateTime >= startDate && m.RequestDateTime <= endDate)
                .OrderByDescending(m => m.RequestDateTime)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetCallCountByEndpointAsync(DateTime startDate, DateTime endDate)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(m => m.RequestDateTime >= startDate && m.RequestDateTime <= endDate)
                .GroupBy(m => m.Endpoint)
                .Select(g => new { Endpoint = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Endpoint, x => x.Count);
        }

        public async Task<Dictionary<string, double>> GetAverageResponseTimeByEndpointAsync(DateTime startDate, DateTime endDate)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(m => m.RequestDateTime >= startDate && m.RequestDateTime <= endDate)
                .GroupBy(m => m.Endpoint)
                .Select(g => new { Endpoint = g.Key, AvgTime = g.Average(m => m.ResponseTimeMs) })
                .ToDictionaryAsync(x => x.Endpoint, x => x.AvgTime);
        }

        public async Task<int> GetTotalCallsAsync(DateTime startDate, DateTime endDate)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .CountAsync(m => m.RequestDateTime >= startDate && m.RequestDateTime <= endDate);
        }

        public async Task<double> GetErrorRateAsync(DateTime startDate, DateTime endDate)
        {
            var dbSet = await GetDbSetAsync();
            var total = await dbSet
                .CountAsync(m => m.RequestDateTime >= startDate && m.RequestDateTime <= endDate);

            if (total == 0) return 0;

            var errors = await dbSet
                .CountAsync(m => !m.IsSuccess && m.RequestDateTime >= startDate && m.RequestDateTime <= endDate);

            return (double)errors / total * 100;
        }
    }
}