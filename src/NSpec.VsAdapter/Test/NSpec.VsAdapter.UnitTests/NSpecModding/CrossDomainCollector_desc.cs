﻿using AutofacContrib.NSubstitute;
using FluentAssertions;
using NSpec.VsAdapter.NSpecModding;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSpec.VsAdapter.UnitTests.NSpecModding
{
    [TestFixture]
    [Category("CrossDomainCollector")]
    [Ignore("Test yet to be implemented")]
    public abstract class CrossDomainCollector_desc_base
    {
        protected CrossDomainCollector collector;

        protected AutoSubstitute autoSubstitute;
        protected IAppDomainFactory appDomainFactory;
        protected IMarshalingFactory<ICollectorInvocation, IEnumerable<NSpecSpecification>> marshalingFactory;
        protected MarshalingWrapper<ICollectorInvocation, IEnumerable<NSpecSpecification>> marshalingWrapper;
        protected ITargetAppDomain targetDomain;
        protected ICollectorInvocation collectorInvocation;
        protected Func<ICollectorInvocation, IEnumerable<NSpecSpecification>> outputSelector;
        protected IEnumerable<NSpecSpecification> actualSpecifications;

        protected const string somePath = @".\some\path\to\library.dll";

        [SetUp]
        public virtual void before_each()
        {
            autoSubstitute = new AutoSubstitute();

            appDomainFactory = autoSubstitute.Resolve<IAppDomainFactory>();

            marshalingFactory = autoSubstitute
                .Resolve<IMarshalingFactory<ICollectorInvocation, IEnumerable<NSpecSpecification>>>();

            marshalingWrapper = Substitute.For<MarshalingWrapper<ICollectorInvocation, IEnumerable<NSpecSpecification>>>();

            targetDomain = Substitute.For<ITargetAppDomain>();

            collector = autoSubstitute.Resolve<CrossDomainCollector>();

            collectorInvocation = Substitute.For<ICollectorInvocation>();

            outputSelector = _ => null;
        }

        [TearDown]
        public virtual void after_each()
        {
            autoSubstitute.Dispose();
        }
    }

    public class CrossDomainCollector_when_run_succeeds : CrossDomainCollector_desc_base
    {

        readonly static NSpecSpecification[] someSpecifications = new NSpecSpecification[] 
        { 
            new NSpecSpecification() { SourceFilePath = somePath, FullName = "source-1-spec-A", },
            new NSpecSpecification() { SourceFilePath = somePath, FullName = "source-1-spec-B", },
            new NSpecSpecification() { SourceFilePath = somePath, FullName = "source-1-spec-C", },
        };

        public override void before_each()
        {
            base.before_each();

            appDomainFactory.Create(somePath).Returns(targetDomain);

            marshalingFactory.CreateWrapper(targetDomain).Returns(marshalingWrapper);

            marshalingWrapper.Execute(collectorInvocation, outputSelector).Returns(someSpecifications);

            actualSpecifications = collector.Run(somePath, collectorInvocation, outputSelector);
        }

        [Test]
        public void it_should_return_collected_specifications()
        {
            actualSpecifications.Should().BeEquivalentTo(someSpecifications);
        }
    }

    public abstract class CrossDomainCollector_when_run_fails : CrossDomainCollector_desc_base
    {
        [Test]
        public void it_should_return_empty_spec_list()
        {
            actualSpecifications.Should().BeEmpty();
        }
    }

    public class CrossDomainCollector_when_app_domain_creation_fails : CrossDomainCollector_when_run_fails
    {
        public override void before_each()
        {
            base.before_each();

            appDomainFactory.Create(null).ReturnsForAnyArgs(_ =>
            {
                throw new InvalidOperationException();
            });

            actualSpecifications = collector.Run(somePath, collectorInvocation, outputSelector);
        }
    }

    public class CrossDomainCollector_when_marshal_wrapper_creation_fails : CrossDomainCollector_when_run_fails
    {
        public override void before_each()
        {
            base.before_each();

            appDomainFactory.Create(null).ReturnsForAnyArgs(targetDomain);

            marshalingFactory.CreateWrapper(null).ReturnsForAnyArgs(_ =>
            {
                throw new InvalidOperationException();
            });

            actualSpecifications = collector.Run(somePath, collectorInvocation, outputSelector);
        }
    }

    public class CrossDomainCollector_when_marshaled_execution_fails : CrossDomainCollector_when_run_fails
    {
        public override void before_each()
        {
            base.before_each();

            appDomainFactory.Create(null).ReturnsForAnyArgs(targetDomain);

            marshalingFactory.CreateWrapper(null).ReturnsForAnyArgs(marshalingWrapper);

            marshalingWrapper.Execute(null, null).ReturnsForAnyArgs(_ =>
            {
                throw new InvalidOperationException();
            });

            actualSpecifications = collector.Run(somePath, collectorInvocation, outputSelector);
        }
    }
}