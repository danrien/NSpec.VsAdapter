using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace NSpec.VsAdapter.Settings
{
    [SettingsName(AdapterSettings.RunSettingsXmlNode)]
    public class AdapterSettingsProvider : IAdapterSettingsProvider
    {
        public AdapterSettings Settings { get; private set; }

        public void Load(XmlReader reader)
        {
            if (reader == null)
            {
                Settings = new AdapterSettings();
                return;
            }

            try
            {
                if (reader.Read() && reader.Name == AdapterSettings.RunSettingsXmlNode)
                {
                    // store settings locally
                    Settings = AdapterSettings.FromXml(XDocument.Load(reader));
                }
                else
                {
                    Settings = new AdapterSettings();
                }
            }
            catch (Exception)
            {
                // Swallow exception, probably cannot even log at this time

                Settings = new AdapterSettings();
            }
        }
    }
}
