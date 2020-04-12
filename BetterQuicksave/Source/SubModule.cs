using System;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.CampaignSystem;

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

        private static bool load = false;

        protected override void OnApplicationTick(float dt)
        {
            if(load) {
                if(GameStateManager.Current.ActiveState is MapState) {
                    load = false;
                    if(Mission.Current != null) {
                        InformationManager.DisplayMessage(new InformationMessage("Mission is not null, failed to quickload!", Colors.Red));
                    } else {
                        QuicksaveManager.LoadLatestQuicksave();
                    }
                }
            } else
            if (Input.IsKeyReleased(Config.QuickloadKey) && QuicksaveManager.CanQuickload)
            {
				if(Mission.Current != null) {
					Mission.Current.RetreatMission();
				}
				load = true;
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
