using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using BetterQuicksave.Utils;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace BetterQuicksave
{
    [XmlRoot("BetterQuicksaveConfig")]
    public class Config
    {
        public static ModuleInfo ModInfo => Instance.InstanceModInfo;
        public static string QuicksavePrefix => 
            Regex.Replace(Instance.InstanceQuicksavePrefix, @"[^\w\-. ]", "");
        public static int MaxQuicksaves => Instance.InstanceMaxQuicksaves;
        public static bool MultipleQuicksaves => MaxQuicksaves > 1;
        public static InputKey QuickloadKey => (InputKey)Instance.InstanceQuickloadKey;
        public static bool PerCharacterSaves => Instance.InstancePerCharacterSaves;

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
        [XmlElement("QuicksavePrefix")]
        public string InstanceQuicksavePrefix { get; set; } = "quicksave_";
        [XmlElement("QuickloadKey")]
        public int InstanceQuickloadKey { get; set; } = (int)InputKey.F9;
        [XmlElement("PerCharacterSaves")]
        public bool InstancePerCharacterSaves { get; set; }
        
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
