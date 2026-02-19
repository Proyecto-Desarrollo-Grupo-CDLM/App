using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MundiFavs.CitySearch;
using MundiFavs.Application.Tests.Destinos;
using MundiFavs.Application.Tests.CitySearch;

namespace MundiFavs.EntityFrameworkCore.Applications.CitySearch;

[Collection(MundiFavsTestConsts.CollectionDefinitionName)]
public class EfCoreDestinoAppService_Tests : CiudadesAppService_Tests<MundiFavsEntityFrameworkCoreTestModule>
{

}