using System.Reflection;

namespace NSpec.VsAdapter
{
    public class AdapterInfo : IAdapterInfo
    {
        public string Name => "NSpec VS Adapter";

        public string Version => typeof(AdapterInfo).GetTypeInfo().Assembly.GetName().Version.ToString();
    }
}
