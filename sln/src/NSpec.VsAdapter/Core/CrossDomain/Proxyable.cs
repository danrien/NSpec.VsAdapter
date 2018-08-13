using System;
#if NET452
using System.Runtime.Remoting;
#endif

namespace NSpec.VsAdapter.Core.CrossDomain
{
    public abstract class Proxyable :
#if NET452
        MarshalByRefObject,
#endif
    IDisposable
    {
        #if NET452
        public override object InitializeLifetimeService()
        {
            // Claim an infinite lease lifetime by returning null here. 
            // To prevent memory leaks as a side effect, instance creators 
            // *must* Dispose() in order to explicitly end the lifetime.

            return null;
        }
        #endif

        // see https://github.com/fixie/fixie/blob/master/src/Fixie/Execution/LongLivedMarshalByRefObject.cs

        public virtual void Dispose() // made virtual to allow test mocking
        {
            #if NET452
            RemotingServices.Disconnect(this);
            #endif
        }
    }
}