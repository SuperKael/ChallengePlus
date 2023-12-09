using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("Planets")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_GameScript_Planets
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            ChallengePlus.SelectedChallengeLevel = GameScript.challengeLevel;
        }
    }
}