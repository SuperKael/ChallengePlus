using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System.Collections;

namespace Softcore.Patches
{
    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("Retry")]
    static class Patch_GameScript_Retry
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [HarmonyPrefix]
        public static bool Prefix(GameScript __instance, ref int[] ___combatChips, ref int ___deathCounter, ref int ___readyPlayers, ref bool ___savingGame, ref int ___droidCount, ref bool ___menuDeathing)
        {
            MonoBehaviour.print("Retry() (Patched)");
            GameScript.poison = 0;
            for (int i = 0; i < 38; i++)
            {
                ___combatChips[i] = 0;
            }
            __instance.SaveGame(0);
            GameScript.dead = false;
            GameScript.curPlanet = 0;
            GameScript.curPlanetTrue = 0;
            __instance.inventoryBar.SetActive(value: true);
            GameScript.retrying = false;
            ___deathCounter = 0;
            __instance.menuDeath.SetActive(value: false);
            ___readyPlayers = 0;
            Camera.main.SendMessage("DeathCamera0");
            if (Network.isServer)
            {
                __instance.ResetStatics();
            }
            Time.timeScale = 1f;
            Network.minimumAllocatableViewIDs = 500;
            ___savingGame = false;
            GameScript.dead = false;
            Menuu.musicLevel = PreviewLabs.PlayerPrefs.GetInt("musicLevel");
            Menuu.soundLevel = PreviewLabs.PlayerPrefs.GetInt("soundLevel");
            __instance.txtMusic[0].text = string.Empty + Menuu.musicLevel;
            __instance.txtMusic[1].text = __instance.txtMusic[0].text;
            __instance.txtSound[0].text = string.Empty + Menuu.soundLevel;
            __instance.txtSound[1].text = __instance.txtSound[0].text;
            GameScript.objectivesComplete = 0;
            if (GameScript.playerLevel == 0)
            {
                GameScript.playerLevel = 1;
            }
            GameScript.isTown = false;
            for (int i = 0; i < 10; i++)
            {
                GameScript.teleporter[i] = 0;
            }
            __instance.StartCoroutine((IEnumerator) typeof(GameScript).GetMethod("ReplenishHP", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { }));
            __instance.StartCoroutine((IEnumerator) typeof(GameScript).GetMethod("ReplenishMana", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { }));
            __instance.UpdateEnergy();
            __instance.StartCoroutine((IEnumerator) typeof(GameScript).GetMethod("ReplenishEnergy", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { }));
            ___droidCount = (int) typeof(GameScript).GetMethod("GetActiveDroidCount", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { });
            GameScript.hp = 1;
            GameScript.energy = GameScript.maxenergy;
            GameScript.portalLevel = PreviewLabs.PlayerPrefs.GetInt("portalLevel" + Menuu.curChar);
            if (Menuu.curName == string.Empty)
            {
                Menuu.curName = "Seanzilla";
            }
            MenuScript.playerAppearance.GetComponent<NetworkView>().RPC("UA", RPCMode.AllBuffered, GameScript.equippedIDs, 0, GameScript.dead);
            for (int i = 0; i < 11; i++)
            {
                __instance.deathStat[i].SetActive(value: false);
            }
            typeof(GameScript).GetMethod("UpdateDroids", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { });
            __instance.UpdateHP();
            __instance.Flush();
            ___menuDeathing = false;
            return false;
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}