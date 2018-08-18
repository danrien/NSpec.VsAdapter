using AutofacContrib.NSubstitute;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using NSpec.VsAdapter.Settings;
using NSubstitute;
using NUnit.Framework;

namespace NSpec.VsAdapter.UnitTests.Settings
{
    [TestFixture]
    [Category("SettingsRepository")]
    public abstract class SettingsRepository_desc_base : TestingContext
    {
        protected SettingsRepository repository;

        protected IDiscoveryContext discoveryContext;
        protected IRunSettings runSettings;

        [SetUp]
        public override void before_each()
        {
		    repository = GetFixtureFor<SettingsRepository>();

            discoveryContext = GetSubstituteFor<IDiscoveryContext>();
            runSettings = GetSubstituteFor<IRunSettings>();
            
            discoveryContext.RunSettings.Returns(runSettings);
        }
    }

    public class SettingsRepository_when_succeeding : SettingsRepository_desc_base
    {
        readonly AdapterSettings someSettings = new AdapterSettings()
        {
            LogLevel = someLogLevel,
        };

        const string someLogLevel = "whatever value";

        public override void before_each()
        {
            base.before_each();

            var settingsProvider = GetFixtureFor<IAdapterSettingsProvider>();

            runSettings.GetSettings(AdapterSettings.RunSettingsXmlNode).Returns(settingsProvider);

            settingsProvider.Settings.Returns(someSettings);
        }

        [Test]
        public void it_should_pass_provider_settings()
        {
            var expected = someSettings;
            var actual = repository.Load(discoveryContext);

            actual.ShouldBeEquivalentTo(expected);
        }
    }

    public abstract class SettingsRepository_when_failing : SettingsRepository_desc_base
    {
        [Test]
        public void it_should_return_empty_settings()
        {
            var expected = new AdapterSettings();
            var actual = repository.Load(discoveryContext);

            actual.ShouldBeEquivalentTo(expected, this.GetType().Name);
        }
    }

    public class SettingsRepository_when_getsettings_fails : SettingsRepository_when_failing
    {
        public override void before_each()
        {
            base.before_each();

            runSettings.GetSettings(Arg.Any<string>()).Returns(_ => { throw new DummyTestException(); });
        }
    }

    public class SettingsRepository_when_settings_provider_has_wrong_type : SettingsRepository_when_failing
    {
        public override void before_each()
        {
            base.before_each();

            var wrongProvider = Substitute.For<ISettingsProvider>();

            runSettings.GetSettings(Arg.Any<string>()).Returns(wrongProvider);
        }
    }
}
