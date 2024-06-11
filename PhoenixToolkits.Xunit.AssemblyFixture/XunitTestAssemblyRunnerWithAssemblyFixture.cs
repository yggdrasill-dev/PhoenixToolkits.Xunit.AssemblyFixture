using Xunit.Abstractions;
using Xunit.Sdk;

namespace PhoenixToolkits.Xunit.AssemblyFixture;

public class XunitTestAssemblyRunnerWithAssemblyFixture : XunitTestAssemblyRunner
{
	private readonly Dictionary<Type, object> m_AssemblyFixtureMappings = new();
	private readonly bool m_IsParallelAllCases = false;

	public XunitTestAssemblyRunnerWithAssemblyFixture(
		ITestAssembly testAssembly,
		IEnumerable<IXunitTestCase> testCases,
		IMessageSink diagnosticMessageSink,
		IMessageSink executionMessageSink,
		ITestFrameworkExecutionOptions executionOptions)
		: base(
			  testAssembly,
			  testCases,
			  diagnosticMessageSink,
			  executionMessageSink,
			  executionOptions)
	{
		var testFrameworkAttr = ((IReflectionAssemblyInfo)TestAssembly.Assembly).Assembly
			.GetCustomAttributes(typeof(AssemblyFixtureTestFrameworkAttribute), true)
			.OfType<AssemblyFixtureTestFrameworkAttribute>()
			.FirstOrDefault();

		if (testFrameworkAttr is not null)
			m_IsParallelAllCases = testFrameworkAttr.ParallelAllCases;
	}

	protected override async Task AfterTestAssemblyStartingAsync()
	{
		// Let everything initialize
		await base.AfterTestAssemblyStartingAsync();

		// Go find all the AssemblyFixtureAttributes adorned on the test assembly
		Aggregator.Run(() =>
		{
			var fixtureTypes = ((IReflectionAssemblyInfo)TestAssembly.Assembly).Assembly
				.GetTypes()
				.Where(type => type.GetCustomAttributes(
					typeof(AssemblyFixtureAttribute),
					false).Cast<AssemblyFixtureAttribute>().Any())
				.ToList();

			// Instantiate all the fixtures
			foreach (var fixtureType in fixtureTypes)
				m_AssemblyFixtureMappings[fixtureType] = Activator.CreateInstance(fixtureType)!;
		});
	}

	protected override Task BeforeTestAssemblyFinishedAsync()
	{
		// Make sure we clean up everybody who is disposable, and use Aggregator.Run to isolate Dispose failures
		foreach (var disposable in m_AssemblyFixtureMappings.Values.OfType<IDisposable>())
			Aggregator.Run(disposable.Dispose);

		return base.BeforeTestAssemblyFinishedAsync();
	}

	protected override Task<RunSummary> RunTestCollectionAsync(
		IMessageBus messageBus,
		ITestCollection testCollection,
		IEnumerable<IXunitTestCase> testCases,
		CancellationTokenSource cancellationTokenSource)
		=> new XunitTestCollectionRunnerWithAssemblyFixture(
			m_AssemblyFixtureMappings,
			testCollection,
			testCases,
			DiagnosticMessageSink,
			messageBus,
			TestCaseOrderer,
			new ExceptionAggregator(Aggregator),
			cancellationTokenSource,
			m_IsParallelAllCases).RunAsync();
}
