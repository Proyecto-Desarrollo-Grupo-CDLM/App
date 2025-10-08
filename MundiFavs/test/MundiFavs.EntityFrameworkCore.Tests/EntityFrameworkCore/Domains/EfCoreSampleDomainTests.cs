using MundiFavs.Samples;
using Xunit;

namespace MundiFavs.EntityFrameworkCore.Domains;

[Collection(MundiFavsTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MundiFavsEntityFrameworkCoreTestModule>
{

}
