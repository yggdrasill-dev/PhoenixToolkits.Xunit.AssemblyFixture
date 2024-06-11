using PhoenixToolkits.Xunit.AssemblyFixture;

[assembly: AssemblyFixtureTestFramework]

namespace PhoenixToolkits.Xunit.AssemblyFixture.UnitTests;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1041:Fixture arguments to test classes must have fixture sources", Justification = "<暫止>")]
public class UsageTests(StubAssemblyFixture instance)
{
	[Fact]
	public void AssemblyFixture使用方法()
	{
		Assert.NotNull(instance);
		Assert.Equal(1, StubAssemblyFixture.InstanceCount);
	}
}
