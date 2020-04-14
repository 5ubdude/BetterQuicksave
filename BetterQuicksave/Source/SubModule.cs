using System;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;

namespace BetterQuicksave
{
    public class SubModule : MBSubModuleBase
    {
        public static event Action OnGameInitFinishedEvent;
        public static event Action OnGameEndEvent;
        
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
                QuicksaveManager.Init();
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
            
            OnGameInitFinishedEvent?.Invoke();
        }

        public override void OnGameEnd(Game game)
        {
            OnGameEndEvent?.Invoke();
        }

        protected override void OnApplicationTick(float dt)
        {
            if (Input.IsKeyReleased(Config.QuickloadKey) && QuicksaveManager.CanQuickload)
            {
                QuicksaveManager.LoadLatestQuicksave();
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
