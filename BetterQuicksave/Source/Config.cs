using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using BetterQuicksave.Utils;
using TaleWorlds.Library;

namespace BetterQuicksave
{
    [XmlRoot("BetterQuicksaveConfig")]
    public class Config
    {
        public const string QuicksavePrefix = "quicksave_";
        public static string QuicksaveNamePattern => $@"^{QuicksavePrefix}(\d{{3}})$";
        public static ModuleInfo ModInfo => Instance.InstanceModInfo;
        public static int MaxQuicksaves => Instance.InstanceMaxQuicksaves;

        private static readonly string ModBasePath =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", ".."));
        private static readonly string ConfigFilename =
            Path.Combine(ModBasePath, "ModuleData", "BetterQuicksaveConfig.xml");

        private static Config _instance;
        private static Config Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = LoadConfig();
                return _instance;
            }
        }

        private ModuleInfo InstanceModInfo { get; } = new ModuleInfo();
        [XmlElement("MaxQuicksaves")]
        public int InstanceMaxQuicksaves { get; set; } = 3;

        private Config() { }
        
        private static Config LoadConfig()
        {
            Config config = DeserializeConfig();
            if (config == null)
            {
                SerializeConfig();
                config = DeserializeConfig();
            }

            config.InstanceModInfo.Load(new DirectoryInfo(ModBasePath).Name);
            return config;
        }

        private static void SerializeConfig()
        {
            XmlSerialization.Serialize(ConfigFilename, new Config());
        }
        
        private static Config DeserializeConfig()
        {
            return XmlSerialization.Deserialize<Config>(ConfigFilename);
        }
    }
}
