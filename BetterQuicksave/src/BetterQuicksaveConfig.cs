using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace BetterQuicksave
{
    public class BetterQuicksaveConfig
    {
        [XmlIgnore]
        public string QuicksavePrefix => "quicksave_";
        [XmlIgnore]
        public string QuicksaveNamePattern => $@"^{QuicksavePrefix}(\d{{3}})$";
        public int MaxQuicksaves { get; set; } = 3;

        private static string ConfigFilename => 
            $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/../../ModuleData/BetterQuicksaveConfig.xml";

        public static BetterQuicksaveConfig LoadConfig()
        {
            var betterQuicksaveConfig = DeserializeConfig<BetterQuicksaveConfig>(ConfigFilename);
            if (betterQuicksaveConfig == null)
            {
                SerializeConfig(ConfigFilename, new BetterQuicksaveConfig());
            }
            return DeserializeConfig<BetterQuicksaveConfig>(ConfigFilename);
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
