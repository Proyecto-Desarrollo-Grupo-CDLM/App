using Microsoft.AspNetCore.Authorization;
using MundiFavs.Destinos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;

namespace MundiFavs.Calificaciones
{
    [Authorize]
    public class CalificacionAppService : CrudAppService<
            Calificacion,
            CalificacionDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateCalificacionDto>,
        ICalificacionAppService
       
        
    {
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IRepository<Calificacion, Guid> _calificacionRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        public Lazy<IObjectMapper> ObjectMapperLazy { get; internal set; }

 
        public CalificacionAppService(
            IRepository<Calificacion, Guid> repository,
            Volo.Abp.Users.ICurrentUser mockCurrentUser,
            IRepository<Destino, Guid> destinoRepository,
            Volo.Abp.Guids.IGuidGenerator mockGuidGenerator)
            : base(repository)
        {
            _calificacionRepository = repository;
            _destinoRepository = destinoRepository;
            _guidGenerator = mockGuidGenerator;
            _calificacionRepository= repository;
        }

        
        [Authorize]
        public async Task<CalificacionDto?> GetMyCalificacionAsync(Guid destinoId)
        {
            var query = await _calificacionRepository.GetQueryableAsync();

            
            var calificacion = query.FirstOrDefault(x => x.DestinoId == destinoId && x.CreatorId == CurrentUser.Id);

            if (calificacion == null)
            {
                return null; 
            }

            return ObjectMapper.Map<Calificacion, CalificacionDto>(calificacion);
        }

        
        [Authorize]
        public override async Task<CalificacionDto> CreateAsync(CreateUpdateCalificacionDto input)
        {
            var destino = await _destinoRepository.GetAsync(input.DestinoId);

            
            var nuevaCalificacion = new Calificacion(
                GuidGenerator.Create(),
                input.Puntuacion,
                input.Comentario,
                input.DestinoId,
                CurrentUser.Id ?? Guid.Empty
            );

            await _calificacionRepository.InsertAsync(nuevaCalificacion, autoSave: true);

            
            await ActualizarPromedioDestinoAsync(input.DestinoId);

            return ObjectMapper.Map<Calificacion, CalificacionDto>(nuevaCalificacion);
        }

        
        [Authorize]
        public override async Task<CalificacionDto> UpdateAsync(Guid id, CreateUpdateCalificacionDto input)
        {
            
            var calificacion = await _calificacionRepository.GetAsync(id);

            calificacion.ActualizarDatos(input.Puntuacion,input.Comentario);    

            
            await _calificacionRepository.UpdateAsync(calificacion, autoSave: true);

            
            await ActualizarPromedioDestinoAsync(input.DestinoId);

            return ObjectMapper.Map<Calificacion, CalificacionDto>(calificacion);
        }

        private async Task ActualizarPromedioDestinoAsync(Guid destinoId)
        {
            var query = await _calificacionRepository.GetQueryableAsync();

            double nuevoPromedio = 0;

            var calificacionesDestino = query.Where(x => x.DestinoId == destinoId);

            if (calificacionesDestino.Any())
            {
                nuevoPromedio = calificacionesDestino.Average(x => (double)x.Estrellas);
            }

            
            var destino = await _destinoRepository.GetAsync(destinoId);
            destino.SetPuntuacion(nuevoPromedio);

            await _destinoRepository.UpdateAsync(destino);
        }
    

        [Authorize]
        
        public async Task<CalificacionDto> UpdateCalificacionAsync(Guid id, UpdateCalificacionDto input)
        {
          
            var calificacion = await _calificacionRepository.GetAsync(id);

            
            if (calificacion.UserId != CurrentUser.Id)
            {
                throw new AbpAuthorizationException("No está autorizado a modificar esta calificación. Solo el propietario puede hacerlo.");
            }

            
            calificacion.Update(input.Estrellas, input.Comentario);

            
            var updatedCalificacion = await _calificacionRepository.UpdateAsync(calificacion);

            
            return ObjectMapper.Map<Calificacion, CalificacionDto>(updatedCalificacion);
        }

        [Authorize]
        public override async Task DeleteAsync(Guid id)
        {
         
            var calificacion = await _calificacionRepository.GetAsync(id);

            
            if (calificacion.UserId != CurrentUser.Id)
            {
                throw new AbpAuthorizationException("No está autorizado a eliminar esta calificación. Solo el propietario puede hacerlo.");
            }

            
            await _calificacionRepository.DeleteAsync(id);
        }
    }

}



