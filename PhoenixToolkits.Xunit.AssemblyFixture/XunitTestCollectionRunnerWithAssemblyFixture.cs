﻿using Xunit.Abstractions;
using Xunit.Sdk;

namespace PhoenixToolkits.Xunit.AssemblyFixture;

public class XunitTestCollectionRunnerWithAssemblyFixture : XunitTestCollectionRunner
{
	private readonly Dictionary<Type, object> m_AssemblyFixtureMappings;
	private readonly IMessageSink m_DiagnosticMessageSink;

	public XunitTestCollectionRunnerWithAssemblyFixture(
		Dictionary<Type, object> assemblyFixtureMappings,
		ITestCollection testCollection,
		IEnumerable<IXunitTestCase> testCases,
		IMessageSink diagnosticMessageSink,
		IMessageBus messageBus,
		ITestCaseOrderer testCaseOrderer,
		ExceptionAggregator aggregator,
		CancellationTokenSource cancellationTokenSource)
		: base(
			  testCollection,
			  testCases,
			  diagnosticMessageSink,
			  messageBus,
			  testCaseOrderer,
			  aggregator,
			  cancellationTokenSource)
	{
		m_AssemblyFixtureMappings = assemblyFixtureMappings;
		m_DiagnosticMessageSink = diagnosticMessageSink;
	}

	protected override Task<RunSummary> RunTestClassAsync(
		ITestClass testClass,
		IReflectionTypeInfo @class,
		IEnumerable<IXunitTestCase> testCases)
	{
		// Don't want to use .Concat + .ToDictionary because of the possibility of overriding types,
		// so instead we'll just let collection fixtures override assembly fixtures.
		var combinedFixtures = new Dictionary<Type, object>(m_AssemblyFixtureMappings);
		foreach (var kvp in CollectionFixtureMappings)
			combinedFixtures[kvp.Key] = kvp.Value;

		// We've done everything we need, so let the built-in types do the rest of the heavy lifting
		return new XunitTestClassRunner(
			testClass,
			@class,
			testCases,
			m_DiagnosticMessageSink,
			MessageBus,
			TestCaseOrderer,
			new ExceptionAggregator(Aggregator),
			CancellationTokenSource,
			combinedFixtures).RunAsync();
	}
}
