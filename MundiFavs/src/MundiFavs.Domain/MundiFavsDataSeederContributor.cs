using System;
using System.Threading.Tasks;
using MundiFavs.Destinos;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace MundiFavs;

public class MundiFavsDataSeederContributor
    : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Destino, Guid> _destinoRepository;

    public MundiFavsDataSeederContributor(IRepository<Destino, Guid> destinoRepository)
    {
        _destinoRepository = destinoRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _destinoRepository.GetCountAsync() <= 0)
        {
            await _destinoRepository.InsertAsync(
                new Destino(
                    Guid.NewGuid(),
                    "Casa de Urquiza",
                    "Argentina",
                    "Concepcion del Uruguay",
                    86001,
                    new Coordenadas(-32.4833M, -58.2333M),
                    new Uri("https://media.lacapital.com.ar/p/27eb99e3df66432aa5efedae39aafa0c/adjuntos/205/imagenes/029/268/0029268240/642x0/smart/urquizajpg.jpg")

                    ),


               
                autoSave: true
            );

            await _destinoRepository.InsertAsync(
                new Destino(
                    Guid.NewGuid(),
                    "Florencio Sanchez",
                    "Uruguay",
                    "Paysandu",
                    121843,
                    new Coordenadas(-32.3213M, -58.0772M),
                    new Uri("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRfPUYCuYkP-nQoVvlAHqrmgvxL2r74cK4mCA&s")
                ),
                autoSave: true
            );
        }
    }
}
