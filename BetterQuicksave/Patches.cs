using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.Core;
using HarmonyLib;

namespace BetterQuicksave
{
    public static class Patches
    {
        [HarmonyPatch(typeof(MBSaveLoad), "QuickSaveCurrentGame")]
        public class QuickSaveCurrentGamePatch
        {
            static readonly MethodInfo OverwriteSaveFile = AccessTools.Method(typeof(MBSaveLoad), "OverwriteSaveFile");

            static readonly MethodInfo GetQuicksaveName =
                SymbolExtensions.GetMethodInfo(() => QuicksaveManager.GetQuicksaveName());

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.opcode == OpCodes.Call && instruction.operand == OverwriteSaveFile)
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                        yield return new CodeInstruction(OpCodes.Call, GetQuicksaveName);
                    }

                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(MBSaveLoad), "IsSaveFileNameReserved")]
        public class IsSaveFileNameReservedPatch
        {
            static void Postfix(ref bool __result, string name)
            {
                __result = QuicksaveManager.IsValidName(name) || __result;
            }
        }
    }
}
