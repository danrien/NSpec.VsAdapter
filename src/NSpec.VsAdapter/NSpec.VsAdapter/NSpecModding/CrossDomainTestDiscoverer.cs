﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using NSpec.VsAdapter.TestAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSpec.VsAdapter.NSpecModding
{
    public class CrossDomainTestDiscoverer : ICrossDomainTestDiscoverer
    {
        public CrossDomainTestDiscoverer(ICrossDomainCollector crossDomainCollector)
        {
            this.crossDomainCollector = crossDomainCollector;
        }

        public IEnumerable<NSpecSpecification> Discover(string assemblyPath, IOutputLogger logger)
        {
            IEnumerable<NSpecSpecification> specifications;

            try
            {
                logger.Debug(String.Format("Processing container: '{0}'", assemblyPath));

                var collectorInvocation = new CollectorInvocation(assemblyPath);

                specifications = crossDomainCollector.Run(
                    assemblyPath, collectorInvocation, invocation => invocation.Collect());

                logger.Debug(String.Format("Found {0} specs", specifications.Count()));

                return specifications;
            }
            catch (Exception ex)
            {
                // Report problem and return for the next assembly, without crashing the discovery process

                var message = String.Format(
                    "Exception found while discovering tests in source '{0}': {1}", assemblyPath, ex);
                
                logger.Error(message);

                specifications = new NSpecSpecification[0];
            }

            return specifications;
        }

        readonly ICrossDomainCollector crossDomainCollector;
    }
}