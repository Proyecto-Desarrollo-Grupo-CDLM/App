using MundiFavs.Application.Tests.Favoritos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MundiFavs.EntityFrameworkCore.Applications.Notificaciones
{
    [Collection(MundiFavsTestConsts.CollectionDefinitionName)]
    public class EfCoreNotificacionesEventosAppService_Tests : EventoDestino_IntegrationTests<MundiFavsEntityFrameworkCoreTestModule>
    {
    }
}
