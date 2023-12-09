using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using BigNumberCore;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(EnemyScript))]
    [HarmonyPatch("Initialize")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_EnemyScript_Initialize
    {
        [HarmonyAfter("BigNumberCore.BigNumberCore.gadget")]
        [HarmonyPostfix]
        public static void Postfix(EnemyScript __instance, ref int ___exp, ref int ___atk)
        {
            if (!__instance.isEgg)
            {
                ___exp -= GameScript.challengeLevel * 50;
            }
            ___atk = ChallengePlus.ScaleDamageForChallenge(___atk, GameScript.challengeLevel, 7); 
        }
    }
}