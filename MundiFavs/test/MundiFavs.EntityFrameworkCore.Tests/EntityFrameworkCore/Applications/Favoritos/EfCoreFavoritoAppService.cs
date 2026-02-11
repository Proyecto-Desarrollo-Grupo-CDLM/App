using MundiFavs.Application.Tests.Destinos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MundiFavs.Application.Tests.Favoritos;

namespace MundiFavs.EntityFrameworkCore.Applications.Favoritos
{
    [Collection(MundiFavsTestConsts.CollectionDefinitionName)]
    public class EfCoreFavoritoAppService_Tests : FavoritoAppService_Tests <MundiFavsEntityFrameworkCoreTestModule>
    {
    }
}



