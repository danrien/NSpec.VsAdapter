﻿#if NET452
using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;

namespace NSpec.VsAdapter.Core.CrossDomain
{
    public class AppDomainFactory : IAppDomainFactory
    {
        public ITargetAppDomain Create(string binaryPath)
        {
            const string targetDomainName = "NSpec.VsAdapter.AppDomainFactory";
            const Evidence useCurrentDomainEvidence = null;

            string binaryFullPath = Path.GetFullPath(binaryPath);

            string configFilePath = binaryFullPath + ".config";

            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            var targetDomainSetup = new AppDomainSetup()
            {
                ConfigurationFile = configFilePath,
                ApplicationBase = Path.GetDirectoryName(currentAssembly.Location)
            };
            // TODO verify if PrivateBinPath should be set as well

            var appDomain = AppDomain.CreateDomain(targetDomainName, useCurrentDomainEvidence, targetDomainSetup);

            var targetDomain = new TargetAppDomain(appDomain);

            return targetDomain;
        }
    }
}
#endif