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
    [HarmonyPatch(typeof(DroidScript))]
    [HarmonyPatch("Update")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_DroidScript_Update
    {
        public static readonly MethodInfo ApplyDroidMultiplier = typeof(PatchHelper).GetMethod("ApplyDroidMultiplier", BindingFlags.Public | BindingFlags.Static);

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var p = TranspilerHelper.CreateProcessor(instructions, gen);
            var ilRef = p.FindRefByInsn(new CodeInstruction(OpCodes.Ldc_R4, 2f));
            ilRef = p.InjectInsn(ilRef.GetRefByOffset(1), new CodeInstruction(OpCodes.Ldarg_0));
            p.InjectHook(ilRef.GetRefByOffset(1), ApplyDroidMultiplier);
            ilRef = p.FindRefByInsn(new CodeInstruction(OpCodes.Ldc_R4, 4f));
            ilRef = p.InjectInsn(ilRef.GetRefByOffset(1), new CodeInstruction(OpCodes.Ldarg_0));
            p.InjectHook(ilRef.GetRefByOffset(1), ApplyDroidMultiplier);
            return p.Insns;
        }
    }
}