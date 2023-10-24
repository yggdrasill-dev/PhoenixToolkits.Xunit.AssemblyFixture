using PhoenixToolkits.Xunit.AssemblyFixture;

[assembly: AssemblyFixtureTestFramework]

namespace PhoenixToolkits.Xunit.AssemblyFixture.UnitTests;

public class UsageTests
{
	private readonly StubAssemblyFixture m_Instance;

	public UsageTests(StubAssemblyFixture instance)
	{
		m_Instance = instance;
	}

	[Fact]
	public void AssemblyFixture使用方法()
	{
		Assert.NotNull(m_Instance);
		Assert.Equal(1, StubAssemblyFixture.InstanceCount);
	}
}
