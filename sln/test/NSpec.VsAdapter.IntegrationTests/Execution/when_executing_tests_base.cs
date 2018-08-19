﻿using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using NSpec.VsAdapter.IntegrationTests.TestData;
using NSpec.VsAdapter.TestAdapter.Execution;
using NUnit.Framework;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace NSpec.VsAdapter.IntegrationTests.Execution
{
    [TestFixture]
    [Category("Integration.TestExecution")]
    public abstract class when_executing_tests_base
    {
        protected NSpecTestExecutor executor;

        protected CollectingFrameworkHandle handle;
        protected IRunContext runContext;

        [SetUp]
        public virtual void before_each()
        {
            runContext = new EmptyRunContext();

            var consoleLogger = new ConsoleLogger();

            handle = new CollectingFrameworkHandle(consoleLogger);

            executor = new NSpecTestExecutor();
        }

        [TearDown]
        public virtual void after_each()
        {
            if (executor != null) executor.Dispose();
        }

        protected static TestResult MapTestCaseToResult(Dictionary<string, TestOutput> outputByFullNameMap, TestCase testCase)
        {
            var testOutput = outputByFullNameMap[testCase.FullyQualifiedName];

            var testResult = new TestResult(testCase)
            {
                Outcome = testOutput.Outcome,
                ErrorMessage = testOutput.ErrorMessage,
            };

            return testResult;
        }

        protected static EquivalencyAssertionOptions<TestResult> ConfigureTestResultMatching(EquivalencyAssertionOptions<TestResult> opts)
        {
            return opts
                .Including(tr => tr.Outcome)
                .Including(tr => tr.ErrorMessage)
                .Including(tr => tr.TestCase.FullyQualifiedName)
                .Including(tr => tr.TestCase.ExecutorUri)
                .Including(tr => tr.TestCase.Source);
        }
    }
}
