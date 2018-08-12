﻿using Autofac;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;

namespace NSpec.VsAdapter.TestAdapter.Execution
{
    [ExtensionUri(Constants.ExecutorUriString)]
    public class NSpecTestExecutor : ITestExecutor, IDisposable
    {
        // Visual Studio test infrastructure requires a default constructor
        public NSpecTestExecutor()
        {
            var scope = DependencyContainer.Instance.BeginScope();

            disposable = scope;

            this.multiSourceTestExecutorFactory = scope.Resolve<IMultiSourceTestExecutorFactory>();
        }

        public void Dispose()
        {
            disposable.Dispose();
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            multiSourceTestExecutor = multiSourceTestExecutorFactory.Create(sources);

            multiSourceTestExecutor.RunTests(frameworkHandle, runContext);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            multiSourceTestExecutor = multiSourceTestExecutorFactory.Create(tests);

            multiSourceTestExecutor.RunTests(frameworkHandle, runContext);
        }

        public void Cancel()
        {
            multiSourceTestExecutor?.CancelRun();
        }

        IMultiSourceTestExecutor multiSourceTestExecutor;

        readonly IMultiSourceTestExecutorFactory multiSourceTestExecutorFactory;
        readonly IDisposable disposable;
    }
}
