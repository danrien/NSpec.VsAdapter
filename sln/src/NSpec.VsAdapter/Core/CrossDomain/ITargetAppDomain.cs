using System;

namespace NSpec.VsAdapter.Core.CrossDomain
{
    public interface ITargetAppDomain : IDisposable
    {
        object CreateInstanceAndUnwrap(string marshalingAssemblyName, string marshalingTypeName);
    }
}
