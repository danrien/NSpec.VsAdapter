using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NSpec.VsAdapter.Settings
{
    public class AdapterSettings : TestRunSettings, IAdapterSettings
    {
        public AdapterSettings()
            : base(RunSettingsXmlNode)
        {
        }

        public string LogLevel { get; set; } = string.Empty;

        public override XmlElement ToXml()
        {
            var rootElement = new XElement(RunSettingsXmlNode);
            rootElement.Add(new XElement(LogLevelNode));
            
            var document = new XmlDocument();
            
            document.LoadXml(rootElement.ToString());

            return document.DocumentElement;
        }

        public static AdapterSettings FromXml(XDocument document)
        {
            var rootElement = document.Descendants(RunSettingsXmlNode).Single();

            return new AdapterSettings
            {
                LogLevel = rootElement.Element(LogLevelNode)?.Value
            };
        }

        public const string RunSettingsXmlNode = "NSpec.VsAdapter";
        const string LogLevelNode = "LogLevel";
    }
}
