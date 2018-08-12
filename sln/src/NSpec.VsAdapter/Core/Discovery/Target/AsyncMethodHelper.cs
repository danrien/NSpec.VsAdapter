using System;
using System.Linq;
using System.Reflection;

namespace NSpec.VsAdapter.Core.Discovery.Target
{
    public class AsyncMethodHelper
    {
        public AsyncMethodHelper(string assemblyName)
        {
            targetAssembly = Assembly.Load(new AssemblyName(assemblyName));
        }

        public string GetClassNameForAsyncMethod(string className, string methodName)
        {
            var definingType = targetAssembly?.GetType(className);

            if (definingType == null) return null;

            var methodInfo = FindAsyncMethodByName(definingType, methodName);

            if (methodInfo == null) return null;

            var stateMachineAttribute = FindAsyncStateMachineAttribute(methodInfo);

            if (stateMachineAttribute == null) return null;

            var stateMachineTypeProperty = FindAsyncStateMachineTypeProperty(stateMachineAttribute);

            var stateMachineType = stateMachineTypeProperty?.GetValue(stateMachineAttribute, new object[0]) as Type;

            return stateMachineType?.FullName;
        }

        static MethodInfo FindAsyncMethodByName(Type definingType, string methodName)
        {
            var methodInfo = definingType
                // get all instance methods in hierarchy, apart from private methods on base classes
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                // exclude methods defined on base NSpec class
                .Where(info => info.GetBaseDefinition().DeclaringType != typeof(nspec))
                .Where(info => info.Name == methodName)
                .OrderBy(info => info.GetParameters().Length)
                .FirstOrDefault();

            return methodInfo;
        }

        static Attribute FindAsyncStateMachineAttribute(MethodInfo method)
        {
            const string asyncStateMachineAttrName = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";

            return method.GetCustomAttributes(false)
                .Cast<Attribute>()
                .FirstOrDefault(attribute => attribute.GetType().FullName == asyncStateMachineAttrName);
        }

        static PropertyInfo FindAsyncStateMachineTypeProperty(Attribute stateMachineAttribute)
        {
            var typeProperty = stateMachineAttribute.GetType().GetProperty("StateMachineType");

            return typeProperty;
        }

        readonly Assembly targetAssembly;
    }
}
