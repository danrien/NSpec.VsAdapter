﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSpec.VsAdapter.Execution
{
    public class OperatorInvocationFactory : IOperatorInvocationFactory
    {
        public IOperatorInvocation Create(string binaryPath, 
            IExecutionObserver executionObserver, LogRecorder logRecorder)
        {
            return new OperatorInvocation(binaryPath, executionObserver, logRecorder);
        }

        public IOperatorInvocation Create(string binaryPath, string[] exampleFullNames, 
            IExecutionObserver executionObserver, LogRecorder logRecorder)
        {
            return new OperatorInvocation(binaryPath, exampleFullNames, executionObserver, logRecorder);
        }
    }
}