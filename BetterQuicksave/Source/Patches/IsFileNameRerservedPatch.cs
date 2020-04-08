using HarmonyLib;
using TaleWorlds.Core;

namespace BetterQuicksave.Patches
{
    [HarmonyPatch(typeof(MBSaveLoad), "IsSaveFileNameReserved")]
    public class IsSaveFileNameReservedPatch
    {
        private static void Postfix(ref bool __result, string name)
        {
            __result = QuicksaveManager.IsValidQuicksaveName(name) || __result;
        }
    }
}
