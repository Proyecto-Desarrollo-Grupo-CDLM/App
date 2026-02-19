using Xunit;

namespace MundiFavs.EntityFrameworkCore;

[CollectionDefinition(MundiFavsTestConsts.CollectionDefinitionName)]
public class MundiFavsEntityFrameworkCoreCollection : ICollectionFixture<MundiFavsEntityFrameworkCoreFixture>
{

}
