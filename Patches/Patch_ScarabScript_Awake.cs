using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using BigNumberCore;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(ScarabScript))]
    [HarmonyPatch("Awake")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_ScarabScript_Awake
    {
        [HarmonyAfter("BigNumberCore.BigNumberCore.gadget")]
        [HarmonyPostfix]
        public static void Postfix(ScarabScript __instance, ref int ___exp)
        {
            ___exp -= GameScript.challengeLevel * (__instance.isFell ? 0 : 500);
        }
    }
}