using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using System;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(ItemScript))]
    [HarmonyPatch("InitL")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_ItemScript_InitL
    {
        [HarmonyPrefix]
        [HarmonyBefore("GadgetCore.core")]
        public static void Prefix(ItemScript __instance, int[] stats)
        {
            if (__instance.gameObject.name == "i3(Clone)")
            {
                if (stats[0] == 52)
                {
                    stats[1] = ChallengePlus.ScaleCreditsForChallenge(stats[1], GameScript.challengeLevel);
                }
                else if ((ItemRegistry.GetItem(stats[0]).Type & ItemType.LOOT_MASK) == ItemType.MONSTER)
                {
                    stats[1] = ChallengePlus.ScaleResourcesForChallenge(stats[1], GameScript.challengeLevel);
                }
                else if ((ItemRegistry.GetItem(stats[0]).Type & ItemType.LEVELING) == ItemType.LEVELING)
                {
                    stats[4] = Math.Max(stats[4], GameScript.challengeLevel);
                }
            }
        }
    }
}