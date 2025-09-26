using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MundiFavs.Destinos;

public interface IBookAppService :
    ICrudAppService< //Defines CRUD methods
        DestinoDto, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateDestinoDto> //Used to create/update a book
{

}

