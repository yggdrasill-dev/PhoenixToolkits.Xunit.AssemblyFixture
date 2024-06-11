using Xunit.Abstractions;
using Xunit.Sdk;

namespace PhoenixToolkits.Xunit.AssemblyFixture;

internal class ParallelTestMethodRunner : XunitTestMethodRunner
{
	readonly object[] m_ConstructorArguments;
	readonly IMessageSink m_DiagnosticMessageSink;

	public ParallelTestMethodRunner(
		ITestMethod testMethod,
		IReflectionTypeInfo @class,
		IReflectionMethodInfo method,
		IEnumerable<IXunitTestCase> testCases,
		IMessageSink diagnosticMessageSink,
		IMessageBus messageBus,
		ExceptionAggregator aggregator,
		CancellationTokenSource cancellationTokenSource,
		object[] constructorArguments)
		: base(
			testMethod,
			@class,
			method,
			testCases,
			diagnosticMessageSink,
			messageBus,
			aggregator,
			cancellationTokenSource,
			constructorArguments)
	{
		m_ConstructorArguments = constructorArguments;
		m_DiagnosticMessageSink = diagnosticMessageSink;
	}

	// This method has been slightly modified from the original implementation to run tests in parallel
	// https://github.com/xunit/xunit/blob/2.4.2/src/xunit.execution/Sdk/Frameworks/Runners/TestMethodRunner.cs#L130-L142
	protected override async Task<RunSummary> RunTestCasesAsync()
	{
		var summary = new RunSummary();

		var caseTasks = TestCases.Select(RunTestCaseAsync);
		var caseSummaries = await Task.WhenAll(caseTasks).ConfigureAwait(false);

		foreach (var caseSummary in caseSummaries)
		{
			summary.Aggregate(caseSummary);
		}

		return summary;
	}

	protected override async Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
	{
		// Create a new TestOutputHelper for each test case since they cannot be reused when running in parallel
		var args = m_ConstructorArguments.Select(a => a is TestOutputHelper ? new TestOutputHelper() : a).ToArray();

		Task<RunSummary> RunTestAsync() => testCase.RunAsync(
			m_DiagnosticMessageSink,
			MessageBus,
			args,
			new ExceptionAggregator(Aggregator),
			CancellationTokenSource);

		// Respect MaxParallelThreads by using the MaxConcurrencySyncContext if it exists, mimicking how collections are run
		// https://github.com/xunit/xunit/blob/2.4.2/src/xunit.execution/Sdk/Frameworks/Runners/XunitTestAssemblyRunner.cs#L169-L176
		if (SynchronizationContext.Current != null)
		{
			var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			return await Task.Factory.StartNew(
				RunTestAsync,
				CancellationTokenSource.Token,
				TaskCreationOptions.DenyChildAttach | TaskCreationOptions.HideScheduler,
				scheduler)
				.Unwrap()
				.ConfigureAwait(false);
		}

		return await Task.Run(RunTestAsync, CancellationTokenSource.Token).ConfigureAwait(false);
	}
}
