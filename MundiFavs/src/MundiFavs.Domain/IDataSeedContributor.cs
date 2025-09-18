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
                new Destino
                {
                    Nombre = "Casa de Urquiza",
                    Pais = "Argentina",
                    Ciudad = "Concepcion del Uruguay",
                    Poblacion = 86001,
                    ImageUrl = new Uri("https://media.lacapital.com.ar/p/27eb99e3df66432aa5efedae39aafa0c/adjuntos/205/imagenes/029/268/0029268240/642x0/smart/urquizajpg.jpg"),




                },
                autoSave: true
            );

            await _destinoRepository.InsertAsync(
                new Destino
                {
                    Nombre = "Florencio Sanchez",
                    Pais = "Uruguay",
                    Ciudad = "Paysandu",
                    Poblacion = 121843,
                    ImageUrl = new Uri("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRfPUYCuYkP-nQoVvlAHqrmgvxL2r74cK4mCA&s"),

                },
                autoSave: true
            );
        }
    }
}
