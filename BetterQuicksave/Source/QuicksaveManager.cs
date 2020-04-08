﻿using System.Text.RegularExpressions;
using TaleWorlds.Core;

namespace BetterQuicksave
{
    public static class QuicksaveManager
    {
        static int currentQuicksaveNum = GetCurrentQuicksaveNumber();

        public static string GetQuicksaveName()
        {
            if (currentQuicksaveNum >= Config.MaxQuicksaves)
            {
                currentQuicksaveNum = 0;
            }

            string quicksaveName;
            if (Config.MaxQuicksaves > 1)
            {
                quicksaveName = $"{Config.QuicksavePrefix}{++currentQuicksaveNum:000}";
            }
            else
            {
                quicksaveName = Config.QuicksavePrefix;
            }

            return quicksaveName;
        }

        public static bool IsValidName(string name)
        {
            return Regex.IsMatch(name, Config.QuicksaveNamePattern);
        }

        private static int GetCurrentQuicksaveNumber()
        {
            if (Config.MaxQuicksaves <= 1)
            {
                return 0;
            }
            
            SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles();
            foreach (SaveGameFileInfo saveFile in saveFiles)
            {
                Match match = Regex.Match(saveFile.Name, Config.QuicksaveNamePattern);
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
