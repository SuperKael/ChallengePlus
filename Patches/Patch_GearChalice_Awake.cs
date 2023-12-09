using UnityEngine;
using HarmonyLib;
using System.Reflection;
using GadgetCore.API;
using System.Collections;
using GadgetCore.Util;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(GearChalice))]
    [HarmonyPatch("Awake")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_GearChalice_Awake
    {
        [HarmonyPostfix]
        public static bool Prefix(GearChalice __instance, ref int ___value, ref Renderer ___r)
        {
			___r = __instance.icon.GetComponent<Renderer>();
			if (SpawnerScript.lastBiome == 9)
			{
				___value = 0;
				__instance.txtValue[0].text = string.Empty;
				__instance.txtValue[1].text = __instance.txtValue[0].text;
				___r.material = (Material)Resources.Load("mat/trans");
				__instance.icon.SetActive(false);
			}
			else
			{
				___value = ChallengePlus.ScaleGearExpForChallenge((int)(GameScript.cadetValue * (SpawnerScript.curBiome < 50 ? (SpawnerScript.curBiome + 5) * 0.2 : 1)), GameScript.challengeLevel);
				__instance.txtValue[0].text = "+" + ___value;
				__instance.txtValue[1].text = __instance.txtValue[0].text;
				__instance.StartCoroutine(__instance.InvokeMethod("ChaliceShift") as IEnumerator);
			}
			GameScript.cadetValue = 0;
			return false;
		}
    }
}