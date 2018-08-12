using NSpec.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NSpec.VsAdapter.Core.Discovery.Target
{
    public class DiscoveredExampleMapper
    {
        static DiscoveredExampleMapper()
        {
            typeNameToBodyGetterMap = new Dictionary<string, BaseExampleBodyGetter>()
            {
                {
                    typeof(Example).Name,
                    GetExampleBodyInfo
                },
                {
                    typeof(MethodExample).Name,
                    GetMethodExampleBodyInfo
                },
                {
                    typeof(AsyncExample).Name,
                    GetAsyncExampleBodyInfo
                },
                {
                    typeof(AsyncMethodExample).Name,
                    GetAsyncMethodExampleBodyInfo
                },
            };
        }

        public DiscoveredExampleMapper(string binaryPath, IDebugInfoProvider debugInfoProvider)
        {
            this.binaryPath = binaryPath;
            this.debugInfoProvider = debugInfoProvider;
        }

        public DiscoveredExample FromExample(ExampleBase example)
        {
            var methodInfo = GetFunctionBodyInfo(example);

            var specClassName = methodInfo.DeclaringType.FullName;
            var exampleMethodName = methodInfo.Name;

            var navigationData = debugInfoProvider.GetNavigationData(specClassName, exampleMethodName);

            var discoveredExample = new DiscoveredExample()
            {
                FullName = example.FullName(),
                SourceAssembly = binaryPath,
                SourceFilePath = navigationData.FileName,
                SourceLineNumber = navigationData.MinLineNumber,
                Tags = example.Tags.Select(tag => tag.Replace("_", " ")).ToArray(),
            };

            return discoveredExample;
        }

        readonly string binaryPath;
        readonly IDebugInfoProvider debugInfoProvider;
        static readonly Dictionary<string, BaseExampleBodyGetter> typeNameToBodyGetterMap;

        static MethodInfo GetFunctionBodyInfo(ExampleBase example)
        {
            var exampleTypeName = example.GetType().Name;

            var hasGetterForType = typeNameToBodyGetterMap.TryGetValue(exampleTypeName, out var getFunctionBodyInfo);

            if (hasGetterForType)
            {
                var info = getFunctionBodyInfo(example);

                return info;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(example), string.Format("Unexpected example type: {0}", exampleTypeName));
            }
        }

        delegate MethodInfo BaseExampleBodyGetter(ExampleBase baseExample);

        static MethodInfo GetExampleBodyInfo(ExampleBase baseExample)
        {
            // core logic taken from https://github.com/osoftware/NSpecTestAdapter/blob/master/NSpec.TestAdapter/Discoverer.cs

            const string actionPrivateFieldName = "action";

            var example = (Example)baseExample;

            var action = example.GetType()
                .GetField(actionPrivateFieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(example) as Action;

            return action.GetMethodInfo();
        }

        static MethodInfo GetMethodExampleBodyInfo(ExampleBase baseExample)
        {
            // core logic taken from https://github.com/osoftware/NSpecTestAdapter/blob/master/NSpec.TestAdapter/Discoverer.cs

            const string methodInfoPrivateFieldName = "method";

            var example = (MethodExample)baseExample;

            var info = example.GetType()
                .GetField(methodInfoPrivateFieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(example) as MethodInfo;

            return info;
        }

        static MethodInfo GetAsyncExampleBodyInfo(ExampleBase baseExample)
        {
            const string asyncActionPrivateFieldName = "asyncAction";

            var example = (AsyncExample)baseExample;

            var asyncAction = example.GetType()
                .GetField(asyncActionPrivateFieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(example) as Func<Task>;

            return asyncAction.GetMethodInfo();
        }

        static MethodInfo GetAsyncMethodExampleBodyInfo(ExampleBase baseExample)
        {
            const string methodInfoPrivateFieldName = "method";

            var example = (AsyncMethodExample)baseExample;

            var info = example.GetType()
                .GetField(methodInfoPrivateFieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(example) as MethodInfo;

            return info;
        }
    }
}
