using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;


namespace MundiFavs.Destinos;
public class DestinoAppService :
    CrudAppService<
        Destino, //The  Destino entity
        DestinoDto, //Used to show Destino
        Guid, //Primary key of the destino entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateDestinoDto>, //Used to create/update a destino
    IDestinoAppService //implement the IDestinoAppService
{
    public DestinoAppService(IRepository<Destino, Guid> repository)
        : base(repository)
    {

    }
}