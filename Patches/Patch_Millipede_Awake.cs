using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using BigNumberCore;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(Millipede))]
    [HarmonyPatch("Awake")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_Millipede_Awake
    {
        [HarmonyAfter("BigNumberCore.BigNumberCore.gadget")]
        [HarmonyPostfix]
        public static void Postfix(Millipede __instance, ref int ___exp)
        {
            ___exp -= GameScript.challengeLevel * (!__instance.isLava || __instance.isMech || (!__instance.isMykonogre && !__instance.isGlaedria) ? 500 : 0);
        }
    }
}