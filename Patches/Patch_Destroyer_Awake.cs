using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using BigNumberCore;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(Destroyer))]
    [HarmonyPatch("Awake")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_Destroyer_Awake
    {
        [HarmonyAfter("BigNumberCore.BigNumberCore.gadget")]
        [HarmonyPostfix]
        public static void Postfix(ref int ___exp)
        {
            ___exp -= GameScript.challengeLevel * 500;
        }
    }
}