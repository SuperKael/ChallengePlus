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
    [HarmonyPatch]
    [HarmonyGadget("Challenge+")]
    public static class Patch_DroidScript_CheckToBeDone
    {
        public static readonly Type CheckToBeDoneIterator = typeof(DroidScript).GetNestedType("<CheckToBeDone>c__Iterator0", BindingFlags.NonPublic);
        public static readonly MethodInfo ApplyDroidDuration = typeof(PatchHelper).GetMethod("ApplyDroidDuration", BindingFlags.Public | BindingFlags.Static);
        public static readonly FieldInfo droidThis = CheckToBeDoneIterator.GetField("$this", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return CheckToBeDoneIterator.GetMethod("MoveNext", BindingFlags.Public | BindingFlags.Instance);
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var p = TranspilerHelper.CreateProcessor(instructions, gen);
            var ilRef = p.FindRefByInsn(new CodeInstruction(OpCodes.Ldc_R4, 0.4f));
            ilRef = p.InjectLoadField(ilRef.GetRefByOffset(1), droidThis);
            p.InjectHook(ilRef.GetRefByOffset(2), ApplyDroidDuration);
            return p.Insns;
        }
    }
}