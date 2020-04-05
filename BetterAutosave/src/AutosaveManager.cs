using System;
using System.Text.RegularExpressions;
using TaleWorlds.Core;

namespace AutoSaveMod
{
    class AutosaveManager
    {
        public static string GetAutosaveName()
        {
            if (currentAutosaveNum >= Main.Config.MaxAutosaves)
            {
                currentAutosaveNum = 0;
            }
            return string.Format("{0}{1:000}", Main.Config.AutosavePrefix, ++currentAutosaveNum);
        }

        public static bool IsValidName(string name)
        {
            return Regex.IsMatch(name, Main.Config.AutosaveNamePattern);
        }

        private static int GetCurrentAutosaveNumber()
        {
            SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles();
            foreach (SaveGameFileInfo saveGameFileInfo in saveFiles)
            {
                Match match = Regex.Match(saveGameFileInfo.Name, Main.Config.AutosaveNamePattern);
                if (match.Success)
                {
                    int result;
                    int.TryParse(match.Groups[1].Value, out result);
                    return result;
                }
            }
            return 0;
        }

        private static int currentAutosaveNum = GetCurrentAutosaveNumber();
    }
}
