using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using BigNumberCore;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(Hivemind))]
    [HarmonyPatch("Start")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_Hivemind_Start
    {
        [HarmonyAfter("BigNumberCore.BigNumberCore.gadget")]
        [HarmonyPostfix]
        public static void Postfix(Hivemind __instance, ref int ___exp)
        {
            ___exp -= GameScript.challengeLevel * (__instance.isCatas ? 0 : 500);
        }
    }
}