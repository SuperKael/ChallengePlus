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
    [HarmonyPatch(typeof(PlayerScript))]
    [HarmonyPatch("TD")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_PlayerScript_TD
    {
        public static readonly MethodInfo ApplyDamageReductionAndDeflection = typeof(PatchHelper).GetMethod("ApplyDamageReductionAndDeflection", BindingFlags.Public | BindingFlags.Static);

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var p = TranspilerHelper.CreateProcessor(instructions, gen);
            var ilRef = p.FindRefByInsn(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)22)).GetRefByOffset(-1);
            var ldArgRef = p.InjectInsn(ilRef, new CodeInstruction(OpCodes.Ldarg_S, p.GetInsn(ilRef.GetRefByOffset(-1)).operand));
            p.InjectHook(ilRef, ApplyDamageReductionAndDeflection);
            p.InjectInsn(ilRef, new CodeInstruction(OpCodes.Starg_S, p.GetInsn(ldArgRef).operand));
            return p.Insns;
        }
    }
}