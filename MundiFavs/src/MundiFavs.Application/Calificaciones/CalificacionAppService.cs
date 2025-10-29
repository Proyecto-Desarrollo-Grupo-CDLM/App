using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users; 

namespace MundiFavs.Calificaciones
{
    [Authorize]
    public class CalificacionAppService : 
        CrudAppService<
            Calificacion, //La entidad Calificacion
            CalificacionDto, //Usado para mostrar Calificacion
            Guid, //Clave primaria de la entidad
            PagedAndSortedResultRequestDto, //Usado para paginación/orden
            CreateUpdateCalificacionDto>, //Usado para crear/actualizar
        ICalificacionAppService
    {
        
         // 1. Campo para guardar el servicio de usuario actual
        private readonly ICurrentUser _currentUser;

        // 2. El constructor debe estar DENTRO de la clase
        //    Inyectamos ICurrentUser además del repositorio (requisito de TP 7)
        public CalificacionAppService(
            IRepository<Calificacion, Guid> repository,
            ICurrentUser currentUser) // Inyectamos el servicio
            : base(repository)
        {
            _currentUser = currentUser; // Asignamos el servicio al campo
        }



        
        
       
    }
}

