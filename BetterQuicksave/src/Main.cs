using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using HarmonyLib;

namespace BetterQuicksave
{
    public class Main : MBSubModuleBase
    {
        public static BetterQuicksaveConfig Config { get; private set; }

        protected override void OnSubModuleLoad()
        {
            Config = BetterQuicksaveConfig.LoadConfig();

            try
            {
                var harmony = new Harmony("com.subdude.bannerlord.betterquicksave");
                harmony.PatchAll();
            }
            catch (Exception exception)
            {
                Debug.PrintError(exception.Message, exception.StackTrace);
                if (exception.InnerException != null)
                {
                    Exception innerException = exception.InnerException;
                    Debug.PrintError(innerException.Message, innerException.StackTrace);
                }
            }
        }
    }
}
