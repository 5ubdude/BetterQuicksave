using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace AutoSaveMod
{
    public class AutosaveConfig
    {
        [XmlIgnore]
        public string AutosavePrefix
        {
            get
            {
                return "autosave_";
            }
        }

        [XmlIgnore]
        public string AutosaveNamePattern
        {
            get
            {
                return "^" + this.AutosavePrefix + "(\\d{3})$";
            }
        }

        public int MaxAutosaves { get; set; } = 5;
        public int AutoSaveTimeInMinutes { get; set; } = 15;
        private static string ConfigFilename
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../ModuleData/AutoSaveConfig.xml";
            }
        }

        public static AutosaveConfig LoadConfig()
        {
            AutosaveConfig autosaveConfig = AutosaveConfig.DeserializeConfig<AutosaveConfig>(AutosaveConfig.ConfigFilename);
            if (autosaveConfig == null)
            {
                AutosaveConfig.SerializeConfig<AutosaveConfig>(AutosaveConfig.ConfigFilename, new AutosaveConfig());
            }
            return AutosaveConfig.DeserializeConfig<AutosaveConfig>(AutosaveConfig.ConfigFilename);
        }

        private static void SerializeConfig<T>(string fileName, T config)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(config.GetType());
            using (StreamWriter streamWriter = File.CreateText(fileName))
            {
                xmlSerializer.Serialize(streamWriter, config);
            }
        }

        private static T DeserializeConfig<T>(string fileName) where T : class
        {
            T result;
            if (!File.Exists(fileName))
            {
                result = default(T);
            }
            else
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                try
                {
                    using (StreamReader streamReader = File.OpenText(fileName))
                    {
                        result = (T)((object)xmlSerializer.Deserialize(streamReader));
                    }
                }
                catch (Exception)
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    result = default(T);
                }
            }
            return result;
        }
    }
}
