using HarmonyLib;
using TaleWorlds.Core;

namespace BetterQuicksave.Patches
{
    [HarmonyPatch(typeof(MBSaveLoad), "IsSaveFileNameReserved")]
    public class IsSaveFileNameReservedPatch
    {
        static void Postfix(ref bool __result, string name)
        {
            __result = QuicksaveManager.IsValidName(name) || __result;
        }
    }
}
