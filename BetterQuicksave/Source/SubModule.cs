using System;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem.Load;

namespace BetterQuicksave
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony harmony;
        private const string harmonyId = "mod.subdude.bannerlord.betterquicksave";
        private Exception onSubModuleLoadException;
        private bool modActive;
        private bool didStartupMessages;

        protected override void OnSubModuleLoad()
        {
            try
            {
                harmony = new Harmony(harmonyId);
                harmony.PatchAll();
                modActive = true;
            }
            catch (Exception exception)
            {
                onSubModuleLoadException = exception;
                harmony?.UnpatchAll(harmonyId);
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (!didStartupMessages)
            {
                DisplayStartupMessages();
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            if (!modActive)
            {
                DisplayModInactiveWarning();
            }
        }

        private static LoadGameResult lgr = null;

        protected override void OnApplicationTick(float dt)
        {
            if(lgr != null) {
                if(GameStateManager.Current.ActiveState is MapState) {
                    if(Mission.Current != null) {
                        InformationManager.DisplayMessage(new InformationMessage("Mission is not null, failed to quickload!", Colors.Red));
                    } else {
                        QuicksaveManager.loadSave(lgr);
                    }
                    lgr = null;
                }
            } else
            if (Input.IsKeyReleased(Config.QuickloadKey) && QuicksaveManager.CanQuickload)
            {
                lgr = QuicksaveManager.GetLatestQuicksave();
                if(lgr == null) {
                    InformationManager.DisplayMessage(new InformationMessage("No quicksaves available."));
                } else {
                    if(lgr.LoadResult.Successful) {
                        if(Mission.Current != null) {
                            Mission.Current.RetreatMission();
                        }
                    } else {
                        InformationManager.DisplayMessage(new InformationMessage("Unable to load quicksave:", 
                        Colors.Yellow));
                        foreach (LoadError loadError in lgr.LoadResult.Errors)
                        {
                            InformationManager.DisplayMessage(new InformationMessage(loadError.Message, Colors.Red));
                        }
                        lgr = null;
                    }
                }
            }
        }

        private void DisplayStartupMessages()
        {
            if (onSubModuleLoadException != null)
            {
                Debug.PrintWarning("Better Quicksave exception:");
                Debug.PrintError(onSubModuleLoadException.Message, onSubModuleLoadException.StackTrace);
                DisplayModInactiveWarning();
            }
            else
            {
                DisplayModLoadedMessage();
            }

            didStartupMessages = true;
        }

        private void DisplayModLoadedMessage()
        {
            var message = new InformationMessage($"Loaded {Config.ModInfo.Name} {Config.ModInfo.Version}",
                Color.FromUint(4282569842U));
            InformationManager.DisplayMessage(message);
        }

        private void DisplayModInactiveWarning()
        {
            var message = new InformationMessage(
                "Better Quicksave failed to initialize. Vanilla quicksave behavior is active!", 
                Color.FromUint(16711680U));
            InformationManager.DisplayMessage(message);
        }
    }
}
