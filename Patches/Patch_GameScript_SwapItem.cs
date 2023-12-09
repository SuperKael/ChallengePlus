using HarmonyLib;
using GadgetCore.API;
using UnityEngine;
using System.Reflection;
using GadgetCore.Util;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("SwapItem")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_GameScript_SwapItem
    {
        [HarmonyPrefix]
        public static bool Prefix(int slot, ref Item ___holdingItem)
        {
            if (___holdingItem.id == ChallengePlus.corruptCharm?.GetID())
            {
                return !ChallengePlus.UseCharm(ref ___holdingItem, slot);
            }
            return true;
        }
    }
}