using GadgetCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ChallengePlus
{
    public static class PatchHelper
    {
        private static readonly FieldInfo inventory = typeof(GameScript).GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance);

        public static float ApplyDroidSpeed(float baseSpeed, DroidScript droid)
        {
            int droidIndex = InstanceTracker.GameScript.droid.ToList().FindIndex(x => x.GetComponent<DroidScript>() == droid);
            if (droidIndex >= 0 && droidIndex < InstanceTracker.GameScript.droid.Length)
            {
                Item droidItem = ((Item[])inventory.GetValue(InstanceTracker.GameScript))[42 + droidIndex];
                double speed = ChallengePlus.GetBonusMiningSpeedMult(droidItem.corrupted);
                droid.e.GetComponent<Animation>()["droidE"].speed = (float)speed;
                return (float)(baseSpeed / speed);
            }
            return baseSpeed;
        }

        public static float ApplyDroidMultiplier(float baseValue, DroidScript droid)
        {
            int droidIndex = InstanceTracker.GameScript.droid.ToList().FindIndex(x => x.GetComponent<DroidScript>() == droid);
            if (droidIndex >= 0 && droidIndex < InstanceTracker.GameScript.droid.Length)
            {
                Item droidItem = ((Item[])inventory.GetValue(InstanceTracker.GameScript))[42 + droidIndex];
                double speed = ChallengePlus.GetBonusMiningSpeedMult(droidItem.corrupted);
                return (float)(baseValue * speed);
            }
            return baseValue;
        }

        public static float ApplyDroidDuration(float baseValue, DroidScript droid)
        {
            int droidIndex = InstanceTracker.GameScript.droid.ToList().FindIndex(x => x.GetComponent<DroidScript>() == droid);
            if (droidIndex >= 0 && droidIndex < InstanceTracker.GameScript.droid.Length)
            {
                Item droidItem = ((Item[])inventory.GetValue(InstanceTracker.GameScript))[42 + droidIndex];
                double speed = ChallengePlus.GetBonusMiningSpeedMult(GameScript.challengeLevel) / ChallengePlus.GetBonusMiningSpeedMult(droidItem.corrupted);
                return (float)(baseValue * speed);
            }
            return baseValue;
        }

        public static double ApplyBonusDamage(double baseValue)
        {
            return baseValue * ChallengePlus.GetBonusDamageMult(((Item[])inventory.GetValue(InstanceTracker.GameScript))[36]);
        }

        public static int ApplyDamageReductionAndDeflection(int baseValue)
        {
            return UnityEngine.Random.value < ChallengePlus.GetBonusDeflectChance(((Item[])inventory.GetValue(InstanceTracker.GameScript))[37]) ? 0 :
                ChallengePlus.ConvertDoubleToInt(Math.Ceiling(baseValue * ChallengePlus.ScaleDamageMultForChallenge(GameScript.challengeLevel) *
                (1 - ChallengePlus.GetBonusDamageReduction(((Item[])inventory.GetValue(InstanceTracker.GameScript))[38]) -
                ChallengePlus.GetBonusDamageReduction(((Item[])inventory.GetValue(InstanceTracker.GameScript))[39]))));
        }

        public static int ApplyBonusRareDropChance(int baseValue)
        {
            return ChallengePlus.ConvertDoubleToInt(baseValue / (1 + ChallengePlus.GetBonusRareDropChance(((Item[])inventory.GetValue(InstanceTracker.GameScript))[40].corrupted) +
                ChallengePlus.GetBonusRareDropChance(((Item[])inventory.GetValue(InstanceTracker.GameScript))[41].corrupted)));
        }
    }
}
