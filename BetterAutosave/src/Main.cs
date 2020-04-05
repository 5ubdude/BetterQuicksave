using System;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using System.Linq;
using Helpers;
using System.Collections.Generic;

namespace AutoSaveMod
{
    public class Main : MBSubModuleBase
    {
        public static AutosaveConfig Config { get; private set; }
        protected override void OnSubModuleLoad()
        {
            Config = AutosaveConfig.LoadConfig();
            try
            {
                Harmony.DEBUG = true;
                Harmony harmony = new Harmony("com.subdude.bannerlord.autosavemod");
                harmony.PatchAll();
            }
            catch (Exception ex)
            {
                Debug.PrintError("HARMONY ERROR!!!!!!!!!!!!!!!");
                Debug.PrintError(ex.Message, ex.StackTrace, 17592186044416UL);
                if (ex.InnerException != null)
                {
                    Exception innerException = ex.InnerException;
                    Debug.PrintError(innerException.Message, innerException.StackTrace, 17592186044416UL);
                }
            }
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);

            CampaignOptions.AutoSaveInMinutes = Config.AutoSaveTimeInMinutes;
        }
    }

}

