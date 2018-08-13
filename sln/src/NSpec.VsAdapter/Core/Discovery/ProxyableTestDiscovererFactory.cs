#if NET452
using NSpec.VsAdapter.Core.CrossDomain;
using NSpec.VsAdapter.Core.Discovery.Target;

namespace NSpec.VsAdapter.Core.Discovery
{
    public class ProxyableTestDiscovererFactory : ProxyableFactory<ProxyableTestDiscoverer, IProxyableTestDiscoverer>
    {
    }
}
#endif