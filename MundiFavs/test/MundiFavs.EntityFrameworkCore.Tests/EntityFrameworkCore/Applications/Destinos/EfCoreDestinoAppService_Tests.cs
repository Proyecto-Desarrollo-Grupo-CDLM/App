using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MundiFavs.Destinos;

namespace MundiFavs.EntityFrameworkCore.Applications.Destinos;

[Collection(MundiFavsTestConsts.CollectionDefinitionName)]
public class EfCoreDestinoAppService_Tests : DestinoAppService_Tests <MundiFavsEntityFrameworkCoreTestModule>
{ 

}