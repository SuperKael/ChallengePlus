using UnityEngine;
using HarmonyLib;
using GadgetCore.API;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("HoverDroid")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_GameScript_HoverDroid
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