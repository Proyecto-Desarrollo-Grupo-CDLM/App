using MundiFavs.Samples;
using Xunit;

namespace MundiFavs.EntityFrameworkCore.Applications;

[Collection(MundiFavsTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MundiFavsEntityFrameworkCoreTestModule>
{

}
