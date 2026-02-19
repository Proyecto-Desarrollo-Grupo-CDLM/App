using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using MundiFavs.Destinos;
using Volo.Abp;

namespace MundiFavs.Favoritos
{
    [Authorize] 
    public class FavoritoAppService : ApplicationService, IFavoritoAppService
    {
        private readonly IRepository<Favorito, Guid> _favoritoRepository;
        private readonly IRepository<Destino, Guid> _destinoRepository;

        public FavoritoAppService(
            IRepository<Favorito, Guid> favoritoRepository,
            IRepository<Destino, Guid> destinoRepository)
        {
            _favoritoRepository = favoritoRepository;
            _destinoRepository = destinoRepository;
        }

        // 6.1 AGREGAR A FAVORITOS
        public async Task AddAsync(CreateFavoritoDto input)
        {
            var currentUser = CurrentUser.Id;

            // Verificamos si ya existe para no dar error de base de datos
            var existe = await _favoritoRepository.AnyAsync(x =>
                x.DestinoId == input.DestinoId && x.CreatorId == currentUser);

            if (existe)
            {
                throw new UserFriendlyException( "El Destino ya pertenece a tus Favoritos"); 
            }

            var favorito = new Favorito(GuidGenerator.Create(), input.DestinoId);

            await _favoritoRepository.InsertAsync(favorito);
        }

        // 6.2 ELIMINAR DE FAVORITOS
        public async Task RemoveAsync(Guid destinoId)
        {
            var currentUser = CurrentUser.Id;

            // Buscamos el favorito específico de este usuario
            var favorito = await _favoritoRepository.FirstOrDefaultAsync(x =>
                x.DestinoId == destinoId && x.CreatorId == currentUser);

            if (favorito != null)
            {
                await _favoritoRepository.DeleteAsync(favorito);
            }
        }

        // 6.3 LISTAR MIS FAVORITOS
        public async Task<List<DestinoDto>> GetListAsync()
        {
            var currentUser = CurrentUser.Id;

            // 1. Obtenemos los IDs de los destinos favoritos del usuario
            // ABP filtra automáticamente por CreatorId si usáramos interfaces específicas,
            // pero aquí lo hacemos manual para ser explícitos.
            var misFavoritosQuery = await _favoritoRepository.GetQueryableAsync();
            var destinoIds = misFavoritosQuery
                .Where(x => x.CreatorId == currentUser)
                .Select(x => x.DestinoId)
                .ToList();

            if (!destinoIds.Any())
            {
                return new List<DestinoDto>();
            }

            // 2. Buscamos la info completa de esos destinos
            // Nota: En una app real con miles de datos, haríamos un JOIN con LINQ,
            // pero esto es seguro y claro para empezar.
            var destinos = await _destinoRepository.GetListAsync(d => destinoIds.Contains(d.Id));

            // 3. Mapeamos a DTO
            return ObjectMapper.Map<List<Destino>, List<DestinoDto>>(destinos);
        }
    }
}