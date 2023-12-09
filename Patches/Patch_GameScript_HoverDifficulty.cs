using UnityEngine;
using HarmonyLib;
using GadgetCore.API;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("HoverDifficulty")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_GameScript_HoverDifficulty
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (ChallengePlus.txtCorruption != null)
            {
                ChallengePlus.txtCorruption.gameObject.SetActive(false);
            }
        }
    }
}