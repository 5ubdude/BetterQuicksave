using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TaleWorlds.Core;

namespace BetterQuicksave.Patches
{
    [HarmonyPatch(typeof(MBSaveLoad), "QuickSaveCurrentGame")]
    public class QuickSaveCurrentGamePatch
    {
        private static readonly MethodInfo OverwriteSaveFile = AccessTools.Method(typeof(MBSaveLoad), "OverwriteSaveFile");

        private static readonly MethodInfo GetQuicksaveName =
            SymbolExtensions.GetMethodInfo(() => QuicksaveManager.GetNewQuicksaveName());

        private static void Prefix()
        {
            QuicksaveManager.OnQuicksave();
        }
        
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
}
