using MundiFavs.Application.Tests.Destinos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MundiFavs.Experiencias;

namespace MundiFavs.EntityFrameworkCore.Applications.Experiencias
{
    [Collection(MundiFavsTestConsts.CollectionDefinitionName)]
    public class EfCoreExperienciaAppService_Tests : ExperienciaAppService_Tests<MundiFavsEntityFrameworkCoreTestModule>
    {
    }
}
