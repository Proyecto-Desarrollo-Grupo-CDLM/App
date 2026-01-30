using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace MundiFavs.Usuarios
{


    [Authorize]
    public class PerfilUsuarioAppService : MundiFavsAppService, IUsuarioAppService
    {
      

       
        protected IIdentityUserRepository UserRepository { get; }
        protected IdentityUserManager UserManager { get; }

        public PerfilUsuarioAppService(IIdentityUserRepository userRepository, IdentityUserManager userManager)
        {
            UserRepository = userRepository;
            UserManager = userManager;
        }


        public async Task<UsuarioPublicoDto> GetPublicProfileAsync(Guid id)
        {
            var user = await UserRepository.GetAsync(id);

            // El mapeo se configura en MundiFavsApplicationAutoMapperProfile
            return ObjectMapper.Map<IdentityUser, UsuarioPublicoDto>(user);
        }



        public async Task<List<UsuarioPublicoDto>> SearchUsersAsync(string filter)
        {
            if (filter.IsNullOrWhiteSpace())
            {
                return new List<UsuarioPublicoDto>();
            }

            // Buscamos usuarios cuyo UserName o Nombre contengan el filtro
            var users = await UserRepository.GetListAsync(filter: filter);

            return ObjectMapper.Map<List<IdentityUser>, List<UsuarioPublicoDto>>(users);
        }


        public virtual async Task DeleteMyAccountAsync()
        {
            var user = await UserManager.FindByIdAsync(CurrentUser.GetId().ToString());

            if (user != null)
            {
                // Al llamar a Delete, ABP detecta que IdentityUser es ISoftDelete
                // y ejecutará un UPDATE en lugar de un DELETE físico.
                var result = await UserManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    throw new UserFriendlyException("No se pudo eliminar la cuenta.");
                }
            }
        }
    }
}
