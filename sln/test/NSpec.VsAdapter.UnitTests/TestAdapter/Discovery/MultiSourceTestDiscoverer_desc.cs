using FluentAssertions;
using NSpec.VsAdapter.Core.Discovery;
using NSpec.VsAdapter.Logging;
using NSpec.VsAdapter.Settings;
using NSpec.VsAdapter.TestAdapter.Discovery;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NSpec.VsAdapter.UnitTests.TestAdapter.Discovery
{
    [TestFixture]
    [Category("MultiSourceTestDiscoverer")]
    public class MultiSourceTestDiscoverer_when_discovering : TestingContext
    {
        MultiSourceTestDiscoverer discoverer;

        List<TestCase> testCases;
        ITestCaseDiscoverySink discoverySink;
        IBinaryTestDiscoverer binaryTestDiscoverer;
        IOutputLogger outputLogger;
        string[] sources;
        Dictionary<string, DiscoveredExample[]> discoveredExamplesBySource;

        public MultiSourceTestDiscoverer_when_discovering()
        {
            string source1 = @".\path\to\some\dummy-library.dll";
            string source2 = @".\other\path\to\another-dummy-library.dll";

            sources = new string[]
            {
                source1,
                source2,
            };

            discoveredExamplesBySource = new Dictionary<string, DiscoveredExample[]>()
            {
                { 
                    source1, 
                    new DiscoveredExample[] 
                    { 
                        new DiscoveredExample() { SourceFilePath = source1, FullName = "source-1-spec-A", },
                        new DiscoveredExample() { SourceFilePath = source1, FullName = "source-1-spec-B", },
                        new DiscoveredExample() { SourceFilePath = source1, FullName = "source-1-spec-C", },
                    }
                },
                { 
                    source2, 
                    new DiscoveredExample[] 
                    { 
                        new DiscoveredExample() { SourceFilePath = source2, FullName = "source-2-spec-A", },
                        new DiscoveredExample() { SourceFilePath = source2, FullName = "source-2-spec-B", },
                    }
                },
            };
        }

        public override void before_each()
        {
            testCases = new List<TestCase>();
            discoverySink = GetSubstituteFor<ITestCaseDiscoverySink>();
            discoverySink.When(sink => sink.SendTestCase(Arg.Any<TestCase>())).Do(callInfo =>
                {
                    var discoveredTestCase = callInfo.Arg<TestCase>();

                    testCases.Add(discoveredTestCase);
                });

            binaryTestDiscoverer = GetSubstituteFor<IBinaryTestDiscoverer>();

            binaryTestDiscoverer.Discover(null, null, null).ReturnsForAnyArgs(callInfo =>
                {
                    string binaryPath = callInfo.Arg<string>();
                    
                    if (sources.Contains(binaryPath))
                    {
                        return discoveredExamplesBySource[binaryPath];
                    }
                    else
                    {
                        return new DiscoveredExample[0];
                    }
                });

            var testCaseMapper = GetSubstituteFor<ITestCaseMapper>();
            testCaseMapper.FromDiscoveredExample(null).ReturnsForAnyArgs(callInfo =>
                {
                    var discoveredExample = callInfo.Arg<DiscoveredExample>();

                    var testCase = new TestCase(discoveredExample.FullName, Constants.ExecutorUri, discoveredExample.SourceFilePath);

                    return testCase;
                });

            var settingsRepository = GetSubstituteFor<ISettingsRepository>();

            var messageLogger = GetSubstituteFor<IMessageLogger>();

            outputLogger = GetSubstituteFor<IOutputLogger>();
            var loggerFactory = GetSubstituteFor<ILoggerFactory>();
            loggerFactory.CreateOutput(messageLogger, Arg.Any<IAdapterSettings>()).Returns(outputLogger);

            discoverer = new MultiSourceTestDiscoverer(sources, binaryTestDiscoverer, testCaseMapper, settingsRepository, loggerFactory);

            discoverer.DiscoverTests(discoverySink, messageLogger, autoSubstitute.Resolve<IDiscoveryContext>());
        }

        [Test]
        public void it_should_pass_message_logger()
        {
            binaryTestDiscoverer.Received().Discover(
                Arg.Any<string>(), outputLogger, Arg.Any<ICrossDomainLogger>());
        }

        [Test]
        public void it_should_send_discovered_test_cases()
        {
            var allSpecs = discoveredExamplesBySource.SelectMany(group => group.Value);

            var expected = allSpecs.Select(spec => spec.FullName);

            var actual = testCases.Select(tc => tc.FullyQualifiedName);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
