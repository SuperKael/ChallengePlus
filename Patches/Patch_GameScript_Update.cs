using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using System;
using GadgetCore.Util;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("Update")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_GameScript_Update
    {
        [HarmonyPostfix]
        public static void Postfix(GameScript __instance, Ray ___ray)
        {
            if (MenuScript.player && !GameScript.pausing && Input.GetMouseButtonDown(0) && GameScript.inventoryOpen && Physics.Raycast(___ray, out RaycastHit hit, 7f))
            {
                if (hit.transform.gameObject.layer != 16 && hit.transform.gameObject.layer != 17)
                {
                    switch (hit.transform.gameObject.name)
                    {
                        case "bSelectChallenge":
                            if (ChallengePlus.SelectedChallengeLevel > ChallengePlus.UnlockedChallengeLevel + 1) return;
                            if (ChallengePlus.SelectedChallengeLevel == ChallengePlus.UnlockedChallengeLevel + 1)
                            {
                                int upgradeCost = Math.Max((ChallengePlus.SelectedChallengeLevel - 1) * 5, 1);
                                if (__instance.GetTrophies() >= upgradeCost)
                                {
                                    __instance.RemoveTrophies(upgradeCost);
                                    __instance.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/purchaseDif"), Menuu.soundLevel / 10f);
                                    PreviewLabs.PlayerPrefs.SetInt("UnlockedChallengeLevel", ++ChallengePlus.UnlockedChallengeLevel);
                                    __instance.InvokeMethod("RefreshChallengeButton");
                                    GadgetCoreAPI.CallCustomRPC("SetChallengeLevel", RPCMode.All, ChallengePlus.SelectedChallengeLevel);
                                }
                            }
                            else
                            {
                                __instance.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/purchaseDif"), Menuu.soundLevel / 10f);
                                GadgetCoreAPI.CallCustomRPC("SetChallengeLevel", RPCMode.All, ChallengePlus.SelectedChallengeLevel);
                            }
                            break;
                        case "bLowerChallenge":
                            if (ChallengePlus.SelectedChallengeLevel <= 0) return;
                            __instance.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/confirm"), Menuu.soundLevel / 10f);
                            ChallengePlus.SelectedChallengeLevel--;
                            __instance.InvokeMethod("RefreshChallengeButton");
                            break;
                        case "bRaiseChallenge":
                            if (ChallengePlus.SelectedChallengeLevel > ChallengePlus.UnlockedChallengeLevel) return;
                            __instance.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/confirm"), Menuu.soundLevel / 10f);
                            ChallengePlus.SelectedChallengeLevel++;
                            __instance.InvokeMethod("RefreshChallengeButton");
                            break;
                    }
                }
            }
        }
    }
}