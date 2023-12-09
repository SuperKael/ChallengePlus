using UnityEngine;
using HarmonyLib;
using GadgetCore.API;
using GadgetCore;

namespace ChallengePlus.Patches
{
    [HarmonyPatch(typeof(PatchMethods))]
    [HarmonyPatch("HoverItem")]
    [HarmonyGadget("Challenge+")]
    public static class Patch_PatchMethods_HoverItem
    {
        [HarmonyPostfix]
        public static void Postfix(Item item)
        {
            if (ChallengePlus.txtCorruption != null)
            {
                if (item.id == ChallengePlus.corruptCharm.GetID())
                {
                    ChallengePlus.txtCorruption.gameObject.SetActive(true);
                    ChallengePlus.txtCorruption.text = "Challenge Lv." + item.corrupted;
                    ChallengePlus.txtCorruption.color = new Color(0.34375f, 0.7265625f, 1f);
                }
                else
                {
                    if (item.corrupted > 0)
                    {
                        ChallengePlus.txtCorruption.gameObject.SetActive(true);
                        switch (ItemRegistry.GetTypeByID(item.id) & ItemType.EQUIP_MASK)
                        {
                            case ItemType.WEAPON & ItemType.EQUIP_MASK:
                                ChallengePlus.txtCorruption.text = string.Format("{0:0.###}% Bonus Damage\nCorruption Level: {1}", (ChallengePlus.GetBonusDamageMult(item) - 1) * 100, item.corrupted);
                                break;
                            case ItemType.OFFHAND & ItemType.EQUIP_MASK:
                                ChallengePlus.txtCorruption.text = string.Format("{0:0.###}% Deflect Chance\nCorruption Level: {1}", ChallengePlus.GetBonusDeflectChance(item) * 100, item.corrupted);
                                break;
                            case ItemType.HELMET & ItemType.EQUIP_MASK:
                                ChallengePlus.txtCorruption.text = string.Format("{0:0.###}% Damage Reduction\nCorruption Level: {1}", ChallengePlus.GetBonusDamageReduction(item) * 100, item.corrupted);
                                break;
                            case ItemType.ARMOR & ItemType.EQUIP_MASK:
                                ChallengePlus.txtCorruption.text = string.Format("{0:0.###}% Damage Reduction\nCorruption Level: {1}", ChallengePlus.GetBonusDamageReduction(item) * 100, item.corrupted);
                                break;
                            case ItemType.RING & ItemType.EQUIP_MASK:
                                ChallengePlus.txtCorruption.text = string.Format("+{0:0.##}x Rare Drop Chance\nCorruption Level: {1}", ChallengePlus.GetBonusRareDropChance(item.corrupted), item.corrupted);
                                break;
                            case ItemType.DROID & ItemType.EQUIP_MASK:
                                ChallengePlus.txtCorruption.text = string.Format("{0:0.###}% Mining Speed\nCorruption Level: {1}", ChallengePlus.GetBonusMiningSpeedMult(item.corrupted) * 100, item.corrupted);
                                break;
                            default:
                                ChallengePlus.txtCorruption.text = string.Format("Corruption Level: {0}", item.corrupted);
                                break;
                        }
                        ChallengePlus.txtCorruption.color = new Color(1f, 0f, 1f);
                    }
                    else
                    {
                        ChallengePlus.txtCorruption.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}