using System;
using System.Text.RegularExpressions;
using BetterQuicksave.Patches;
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
        public static bool CanQuickload => Game.Current?.CurrentState == Game.State.Running && Game.Current.GameType.SupportsSaving;
      
        private static readonly EventListeners eventListeners = new EventListeners();
        private static int NextQuicksaveNumber { get; set; } = 1;
        private static string CurrentPlayerName
        {
            get
            {
                Hero player = Campaign.Current?.MainParty?.LeaderHero;
                return player != null ? $"{player.Name} {player.Clan.Name}" : "No Name";
            }
        }

        private static LoadGameResult CurrentLoadGameResult { get; set; } = null;
        private static string QuicksaveNamePattern
        {
            get
            {
                string playerCharacterName = string.Empty;
                if (Config.PerCharacterSaves && CurrentPlayerName.Length > 0)
                {
                    playerCharacterName = $"{Regex.Escape(CurrentPlayerName)} ";
                }
                string prefix = Regex.Escape(Config.QuicksavePrefix);
                string saveNumber = Config.MaxQuicksaves > 1 ? @"(\d{3})" : string.Empty;

                return $"^{playerCharacterName}{prefix}{saveNumber}$";
            }
        }

        static QuicksaveManager()
        {
            SubModule.OnGameInitFinishedEvent += eventListeners.OnGameInitFinished;
            SubModule.OnGameEndEvent += eventListeners.OnGameEnd;
            SubModule.OnApplicationTickEvent += eventListeners.OnApplicationTick;
            QuickSaveCurrentGamePatch.OnQuicksave += eventListeners.OnQuicksave;
        }

        /// <summary>
        /// Empty method used to invoke constructor, needed in order to setup event listeners at the right time
        /// and to prevent duplicate listeners from accidentally being created.
        /// </summary>
        public static void Init() { }
        
        public static string GetNextQuicksaveName()
        {
            if (NextQuicksaveNumber > Config.MaxQuicksaves)
            {
                NextQuicksaveNumber = 1;
            }

            string characterName = Config.PerCharacterSaves ? $"{CurrentPlayerName} ": string.Empty;
            string saveNum = Config.MultipleQuicksaves ? $"{NextQuicksaveNumber:000}" : string.Empty;

            return $"{characterName}{Config.QuicksavePrefix}{saveNum}";
        }

        public static bool IsValidQuicksaveName(string name)
        {
            return Regex.IsMatch(name, QuicksaveNamePattern);
        }

        public static void Quickload()
        {
            CurrentLoadGameResult = GetLatestQuicksave();
            if (CurrentLoadGameResult == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("No quicksaves available."));
            }
            else
            {
                if (CurrentLoadGameResult.LoadResult.Successful)
                {
                    if (Mission.Current != null)
                    {
                        Mission.Current.RetreatMission();
                    }
                }
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage("Unable to load quicksave:",
                        Colors.Yellow));
                    foreach (LoadError loadError in CurrentLoadGameResult.LoadResult.Errors)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(loadError.Message, Colors.Red));
                    }

                    CurrentLoadGameResult = null;
                }
            }
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

        private static void HandleCurrentLoadGameResult()
        {
            if (GameStateManager.Current.ActiveState is MapState)
            {
                if (Mission.Current != null)
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage("Mission is not null, failed to quickload!", Colors.Red));
                }
                else
                {
                    LoadSave(CurrentLoadGameResult);
                }

                CurrentLoadGameResult = null;
            }
        }

        private static void LoadSave(LoadGameResult lgr)
        {
            ScreenManager.PopScreen();
            GameStateManager.Current.CleanStates(0);
            GameStateManager.Current = Module.CurrentModule.GlobalGameStateManager;
            MBGameManager.StartNewGame(new CampaignGameManager(lgr.LoadResult));
        }

        private static void SetNextQuicksaveNumber()
        {
            SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles();
            foreach (SaveGameFileInfo saveFile in saveFiles)
            {
                Match match = Regex.Match(saveFile.Name, QuicksaveNamePattern);
                if (match.Success)
                {
                    Int32.TryParse(match.Groups[1].Value, out int num);
                    NextQuicksaveNumber = num + 1;
                    return;
                }
            }

            NextQuicksaveNumber = 1;
        }

        private class EventListeners
        {
            public void OnApplicationTick()
            {
                if (CurrentLoadGameResult != null)
                {
                    HandleCurrentLoadGameResult();
                }
            }
            
            private void OnPlayerCharacterChanged(Hero hero, MobileParty party)
            {
                SetNextQuicksaveNumber();
            }

            public void OnQuicksave()
            {
                InformationManager.DisplayMessage(new InformationMessage("Quicksaved."));
                NextQuicksaveNumber++;
            }

            public void OnGameInitFinished()
            {
                if (Campaign.Current != null)
                {
                    CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this,
                        OnPlayerCharacterChanged);
                }
            }

            public void OnGameEnd()
            {
                if (Campaign.Current != null)
                {
                    CampaignEvents.OnPlayerCharacterChangedEvent.ClearListeners(this);
                }
            }
        }
    }
}
