using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using System;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("RefreshChallengeButton")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_GameScript_RefreshChallengeButton
    {
        [HarmonyPrefix]
        public static bool Prefix(GameScript __instance)
        {
            __instance.txtChallengeLevel[0].transform.parent.GetComponent<MeshRenderer>().material = ChallengePlus.SelectedChallengeLevel == ChallengePlus.UnlockedChallengeLevel + 1 ? ChallengePlus.ChallengeButtonWithTrophy : ChallengePlus.ChallengeButton;
            __instance.txtChallengeLevel[0].transform.parent.GetComponent<ButtonMenu>().button = ChallengePlus.SelectedChallengeLevel == ChallengePlus.UnlockedChallengeLevel + 1 ? ChallengePlus.ChallengeButtonWithTrophy : ChallengePlus.ChallengeButton;
            __instance.txtChallengeLevel[0].transform.parent.GetComponent<ButtonMenu>().buttonSelect = ChallengePlus.SelectedChallengeLevel == ChallengePlus.UnlockedChallengeLevel + 1 ? ChallengePlus.ChallengeButtonWithTrophy2 : ChallengePlus.ChallengeButton2;
            
            __instance.txtChallengeLevel[0].text = ChallengePlus.SelectedChallengeLevel > 0 ? "Challenge Lv." + ChallengePlus.SelectedChallengeLevel : "Normal Mode" + (ChallengePlus.SelectedChallengeLevel > ChallengePlus.UnlockedChallengeLevel + 1 ? " (Locked)" : "");
            __instance.txtChallengeCost[0].text = ChallengePlus.SelectedChallengeLevel == ChallengePlus.UnlockedChallengeLevel + 1 ? "" + Math.Max((ChallengePlus.SelectedChallengeLevel - 1) * 5, 1) : "";
            __instance.txtChallengeLevel[1].text = __instance.txtChallengeLevel[0].text;
            __instance.txtChallengeCost[1].text = __instance.txtChallengeCost[0].text;
            return false;
        }
    }
}