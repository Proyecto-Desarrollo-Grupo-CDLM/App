using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Identity; // <--- ESTE ES EL IMPORTANTE
using Volo.Abp.Users;

// BORRA O COMENTA ESTAS LÍNEAS QUE CAUSAN EL CONFLICTO:
// using Microsoft.AspNet.Identity; 
// using Microsoft.AspNetCore.Identity;
// using IdentityUser = Microsoft.AspNetCore.Identity.IdentityUser; <--- ESTA ES LA CULPABLE

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
            // 'user' aquí es de tipo Volo.Abp.Identity.IdentityUser
            var user = await UserRepository.GetAsync(id);

            // Mapeamos explícitamente desde Volo.Abp.Identity.IdentityUser
            return ObjectMapper.Map<Volo.Abp.Identity.IdentityUser, UsuarioPublicoDto>(user);
        }

        public async Task<List<UsuarioPublicoDto>> SearchUsersAsync(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return new List<UsuarioPublicoDto>();
            }

            // 'users' es una lista de Volo.Abp.Identity.IdentityUser
            var users = await UserRepository.GetListAsync(filter: filter);

            return ObjectMapper.Map<List<Volo.Abp.Identity.IdentityUser>, List<UsuarioPublicoDto>>(users);
        }

        public virtual async Task DeleteMyAccountAsync()
        {
            var userId = CurrentUser.GetId();
            var user = await UserManager.FindByIdAsync(userId.ToString());

            if (user != null)
            {
                var result = await UserManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    throw new UserFriendlyException("No se pudo eliminar la cuenta.");
                }
            }
        }
    }
}