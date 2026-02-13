using MundiFavs.External;
using MundiFavs.Tests.Calificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MundiFavs.EntityFrameworkCore.Applications.ApiMetrics
{
    [Collection(MundiFavsTestConsts.CollectionDefinitionName)]
    public class EfCoreApiMetricAppService_Test : ApiMetric_Integration_Tests<MundiFavsEntityFrameworkCoreTestModule>
    {
    }
}
