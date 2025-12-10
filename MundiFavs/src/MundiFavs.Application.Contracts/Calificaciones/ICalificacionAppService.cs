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
    public interface ICalificacionAppService:
         ICrudAppService< //Defines CRUD methods
        CalificacionDto, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateCalificacionDto> //Used to create/update a book
    {
    }
}
