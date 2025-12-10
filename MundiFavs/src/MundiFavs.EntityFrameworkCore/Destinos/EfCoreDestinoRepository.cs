using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MundiFavs.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MundiFavs.Destinos
{
    public class EfCoreDestinoRepository : EfCoreRepository<MundiFavsDbContext, Destino, Guid>, IDestinoRepository
    {
        public EfCoreDestinoRepository(IDbContextProvider<MundiFavsDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Destino>> GetPopularAsync(int maxCount)
        {
            var dbSet = await GetDbSetAsync();

            // PASO 1: Obtener el Ranking (Solo Nombres y Cantidad)
            // Esto genera un SQL muy simple y rápido que SQL Server entiende perfecto.
            var ranking = await dbSet
                .GroupBy(d => new { d.Nombre, d.Pais })
                .Select(g => new
                {
                    Nombre = g.Key.Nombre,
                    Pais = g.Key.Pais,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(maxCount)
                .ToListAsync();

            // Si no hay nada guardado, devolvemos lista vacía
            if (!ranking.Any())
            {
                return new List<Destino>();
            }

            // PASO 2: Recuperar los datos completos (Foto, Coordenadas) para los ganadores
            var destinosCompletos = new List<Destino>();

            foreach (var item in ranking)
            {
                // Por cada ciudad ganadora, traemos UN ejemplo (el primero que encontremos)
                // para poder mostrar su foto y datos en el Frontend.
                var detalleDestino = await dbSet
                    .Where(d => d.Nombre == item.Nombre && d.Pais == item.Pais)
                    .FirstOrDefaultAsync();

                if (detalleDestino != null)
                {
                    destinosCompletos.Add(detalleDestino);
                }
            }

            return destinosCompletos;
        }
    }
}