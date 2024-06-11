using Xunit.Sdk;

namespace PhoenixToolkits.Xunit.AssemblyFixture;

[TestFrameworkDiscoverer("PhoenixToolkits.Xunit.AssemblyFixture.AssemblyFixtureTestFrameworkDiscoverer", "PhoenixToolkits.Xunit.AssemblyFixture")]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class AssemblyFixtureTestFrameworkAttribute : Attribute, ITestFrameworkAttribute
{
	public bool ParallelAllCases { get; set; } = false;
}
