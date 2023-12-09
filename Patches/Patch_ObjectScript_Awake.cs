using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(ObjectScript))]
    [HarmonyPatch("Awake")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_ObjectScript_Awake
    {
        [HarmonyPostfix]
        public static void Postfix(ref int ___hp)
        {
            ___hp = ChallengePlus.ScaleResourcesForChallenge(___hp, GameScript.challengeLevel);
        }
    }
}