using System.Text.RegularExpressions;
using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.MountAndBlade;

namespace BetterQuicksave
{
    public static class QuicksaveManager
    {
        public static bool CanQuickload => Game.Current?.CurrentState == Game.State.Running; //&&
            //GameStateManager.Current.ActiveState is MapState;
        static int currentQuicksaveNum = GetCurrentQuicksaveNumber();

        public static string GetNewQuicksaveName()
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

        public static bool IsValidQuicksaveName(string name)
        {
            return Regex.IsMatch(name, Config.QuicksaveNamePattern);
        }

        public static void LoadLatestQuicksave()
        {
            LoadGameResult loadGameResult = GetLatestQuicksave();
            if (loadGameResult == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("No quicksaves available."));
                return;
            }
            if (!loadGameResult.LoadResult.Successful)
            {
                InformationManager.DisplayMessage(new InformationMessage("Unable to load quicksave:", 
                    Colors.Yellow));
                foreach (LoadError loadError in loadGameResult.LoadResult.Errors)
                {
                    InformationManager.DisplayMessage(new InformationMessage(loadError.Message, Colors.Red));
                }
            }
            else
            {
                ScreenManager.PopScreen();
                GameStateManager.Current.CleanStates(0);
                GameStateManager.Current = Module.CurrentModule.GlobalGameStateManager;
                MBGameManager.StartNewGame(new CampaignGameManager(loadGameResult.LoadResult));
            }
        }

        public static void OnQuicksave()
        {
            InformationManager.DisplayMessage(new InformationMessage("Quicksaved."));
        }

        private static LoadGameResult GetLatestQuicksave()
        {
            SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles();
            foreach (SaveGameFileInfo saveFile in saveFiles)
            {
                if (IsValidQuicksaveName(saveFile.Name))
                {
                    return MBSaveLoad.LoadSaveGameData(saveFile.Name, Utilities.GetModulesNames());
                }
            }

            return null;
        }

        private static int GetCurrentQuicksaveNumber()
        {
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
