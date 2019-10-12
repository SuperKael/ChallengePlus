using UnityEngine;
using HarmonyLib;

namespace Softcore.Patches
{
    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("EraseCharacter")]
    static class Patch_GameScript_EraseCharacter
    {
        [HarmonyPrefix]
        public static bool Prefix(GameScript __instance, int a)
        {
            MonoBehaviour.print("EraseCharacter() (Patched)");
            PreviewLabs.PlayerPrefs.SetInt(a + "hp", 1);
            return false;
        }
    }
}