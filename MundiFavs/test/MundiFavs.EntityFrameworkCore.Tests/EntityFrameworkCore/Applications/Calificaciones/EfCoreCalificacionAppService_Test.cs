using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MundiFavs.Tests.Calificaciones;

namespace MundiFavs.EntityFrameworkCore.Applications.Calificaciones
{
    [Collection(MundiFavsTestConsts.CollectionDefinitionName)]
    public class EfCoreCalificacionAppService_Test: CalificacionAppService_IntegrationTests<MundiFavsEntityFrameworkCoreTestModule>
    {
    }
}

