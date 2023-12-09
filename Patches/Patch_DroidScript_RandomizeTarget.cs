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
    public static class Patch_DroidScript_RandomizeTarget
    {
        public static readonly Type RandomizeTargetIterator = typeof(DroidScript).GetNestedType("<RandomizeTarget>c__Iterator1", BindingFlags.NonPublic);
        public static readonly MethodInfo ApplyDroidSpeed = typeof(PatchHelper).GetMethod("ApplyDroidSpeed", BindingFlags.Public | BindingFlags.Static);
        public static readonly FieldInfo droidThis = RandomizeTargetIterator.GetField("$this", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return RandomizeTargetIterator.GetMethod("MoveNext", BindingFlags.Public | BindingFlags.Instance);
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var p = TranspilerHelper.CreateProcessor(instructions, gen);
            var ilRef = p.FindRefByInsn(new CodeInstruction(OpCodes.Ldc_R4, 0.5f));
            ilRef = p.InjectLoadField(ilRef.GetRefByOffset(1), droidThis);
            p.InjectHook(ilRef.GetRefByOffset(2), ApplyDroidSpeed);
            return p.Insns;
        }
    }
}