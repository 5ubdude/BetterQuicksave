using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TaleWorlds.Core;

namespace AutoSaveMod
{
    public static class Patches
    {
        [HarmonyPatch(typeof(MBSaveLoad), "AutoSaveCurrentGame")]
        public static class AutoSaveCurrentGamePatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
               
                foreach (CodeInstruction instruction in instructions)
                {
                     if (instruction.opcode == OpCodes.Call && instruction.operand as MethodInfo == OverwriteSaveFile)
                     {
                         yield return new CodeInstruction(OpCodes.Pop);
                         yield return new CodeInstruction(OpCodes.Call, GetAutosaveName);
                     }
                     yield return instruction;
                }
            }

            static MethodInfo OverwriteSaveFile = AccessTools.Method(typeof(MBSaveLoad), "OverwriteSaveFile");
            static MethodInfo GetAutosaveName = SymbolExtensions.GetMethodInfo(() => AutosaveManager.GetAutosaveName());
        }

        [HarmonyPatch(typeof(MBSaveLoad), "IsSaveFileNameReserved")]
        public class IsSaveFileNameReservedPatch
        {
            static void Postfix(ref bool __result, string name)
            {
                __result = (AutosaveManager.IsValidName(name) | __result);
            }
        }
    }
}
