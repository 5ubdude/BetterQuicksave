using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.Core;

namespace BetterQuicksave
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony harmony;
        private const string harmonyId = "mod.subdude.bannerlord.betterquicksave";
        private Exception onSubModuleLoadException;
        private bool modActive;

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
            DisplayStartupMessages();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            if (!modActive)
            {
                DisplayModInactiveWarning();
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
