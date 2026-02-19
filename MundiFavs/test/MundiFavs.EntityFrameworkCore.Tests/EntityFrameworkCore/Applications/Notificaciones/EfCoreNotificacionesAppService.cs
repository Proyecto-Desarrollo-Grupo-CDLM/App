using MundiFavs.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MundiFavs.Notificaciones;

namespace MundiFavs.EntityFrameworkCore.Applications.Notificaciones
{

    [Collection(MundiFavsTestConsts.CollectionDefinitionName)]
    public class EfCoreNotificacionesEventosAppService_Tests : NotificacionAppService_Tests<MundiFavsEntityFrameworkCoreTestModule>
    {
    }
}
