using MundiFavs.Application.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MundiFavs.Eventos;

namespace MundiFavs.EntityFrameworkCore.Applications.Eventos
{
    [Collection(MundiFavsTestConsts.CollectionDefinitionName)]
    public class EfCoreNotificacionesEventosAppService_Tests : EventoAppService_Tests<MundiFavsEntityFrameworkCoreTestModule>
    {
    }
}
