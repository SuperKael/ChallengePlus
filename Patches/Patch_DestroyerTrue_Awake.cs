using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using BigNumberCore;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(DestroyerTrue))]
    [HarmonyPatch("Awake")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_DestroyerTrue_Awake
    {
        [HarmonyAfter("BigNumberCore.BigNumberCore.gadget")]
        [HarmonyPostfix]
        public static void Postfix(ref int ___exp)
        {
            ___exp -= GameScript.challengeLevel * 500;
        }
    }
}