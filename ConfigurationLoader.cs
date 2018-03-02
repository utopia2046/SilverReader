using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverReaderApp
{
    using SilverReaderApp.Properties;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;

    class ConfigurationLoader
    {
        public static string _defaultSettingsFilename = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "settings.xml";

        public static void Save(Settings settings, string filename = null)
        {
            if (String.IsNullOrEmpty(filename))
            {
                filename = _defaultSettingsFilename;
            }

            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                xmls.Serialize(sw, settings);
            }
        }

        public static Settings Read(string filename)
        {
            using (StreamReader sw = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                return xmls.Deserialize(sw) as Settings;
            }
        }
    }
}
