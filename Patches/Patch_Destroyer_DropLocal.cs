using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using GadgetCore.API;
using System;
using GadgetCore.Util;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(Destroyer))]
    [HarmonyPatch("DropLocal")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_Destroyer_DropLocal
    {
        public static readonly MethodInfo ApplyBonusRareDropChance = typeof(PatchHelper).GetMethod("ApplyBonusRareDropChance", BindingFlags.Public | BindingFlags.Static);

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var p = TranspilerHelper.CreateProcessor(instructions, gen);
            var refs = p.FindAllRefsByInsns(new List<CodeInstruction>(new CodeInstruction[] {
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ldc_I4_S),
                new CodeInstruction(OpCodes.Call),
                new CodeInstruction(OpCodes.Brtrue)
            }));
            foreach (var ilRef in refs)
            {
                if ((sbyte)p.GetInsn(ilRef.GetRefByOffset(1)).operand == 75) continue;
                p.InjectHook(ilRef.GetRefByOffset(2), ApplyBonusRareDropChance);
            }
            return p.Insns;
        }
    }
}