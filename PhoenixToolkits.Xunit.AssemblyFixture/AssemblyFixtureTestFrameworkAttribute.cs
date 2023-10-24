using Xunit.Sdk;

namespace PhoenixToolkits.Xunit.AssemblyFixture;

[TestFrameworkDiscoverer("PhoenixToolkits.Xunit.AssemblyFixture.AssemblyFixtureTestFrameworkDiscoverer", "PhoenixToolkits.Xunit.AssemblyFixture")]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class AssemblyFixtureTestFrameworkAttribute : Attribute, ITestFrameworkAttribute
{
}
