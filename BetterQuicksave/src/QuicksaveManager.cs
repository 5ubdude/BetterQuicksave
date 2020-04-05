using System.Text.RegularExpressions;
using TaleWorlds.Core;

namespace BetterQuicksave
{
    public static class QuicksaveManager
    {
        static int currentQuicksaveNum = GetCurrentQuicksaveNumber();

        public static string GetQuicksaveName()
        {
            if (currentQuicksaveNum >= Main.Config.MaxQuicksaves)
            {
                currentQuicksaveNum = 0;
            }
            return $"{Main.Config.QuicksavePrefix}{++currentQuicksaveNum:000}";
        }

        public static bool IsValidName(string name)
        {
            return Regex.IsMatch(name, Main.Config.QuicksaveNamePattern);
        }

        static int GetCurrentQuicksaveNumber()
        {
            SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles();
            foreach (SaveGameFileInfo saveFile in saveFiles)
            {
                Match match = Regex.Match(saveFile.Name, Main.Config.QuicksaveNamePattern);
                if (match.Success)
                {
                    int.TryParse(match.Groups[1].Value, out int num);
                    return num;
                }
            }

            return 0;
        }
    }
}
