using Xunit.Abstractions;
using Xunit.Sdk;

namespace PhoenixToolkits.Xunit.AssemblyFixture;

public class AssemblyFixtureTestFrameworkDiscoverer : ITestFrameworkTypeDiscoverer
{
	public Type GetTestFrameworkType(IAttributeInfo attribute)
		=> typeof(XunitTestFrameworkWithAssemblyFixture);
}
