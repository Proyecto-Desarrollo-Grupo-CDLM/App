using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MundiFavs.Usuarios
{
    public interface IUsuarioAppService: IApplicationService
    {
        Task<UsuarioPublicoDto> GetPublicProfileAsync(Guid id);

        Task<List<UsuarioPublicoDto>> SearchUsersAsync(string filter);

        Task DeleteMyAccountAsync();
    }
}

