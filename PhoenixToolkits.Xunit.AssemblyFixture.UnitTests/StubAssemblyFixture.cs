namespace PhoenixToolkits.Xunit.AssemblyFixture.UnitTests;

[AssemblyFixture]
public class StubAssemblyFixture
{
	public static int InstanceCount { get; private set; }

	public StubAssemblyFixture()
	{
		InstanceCount++;
	}
}
