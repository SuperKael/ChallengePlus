using UnityEngine;
using UnityEngine.SceneManagement;
using GadgetCore.API;
using System.Reflection;
using GadgetCore.Util;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System.Runtime.InteropServices;
using System.Globalization;
using GadgetCore;
using BigNumberCore;

namespace ChallengePlus
{
    [Gadget("Challenge+", true, LoadPriority: 100, LoadAfter: new string[] { "BigNumberCore" }, Dependencies: new string[] { "BigNumberCore" })]
    public class ChallengePlus : Gadget<ChallengePlus>
    {
        public const string MOD_VERSION = "2.0"; // Set this to the version of your mod.
        public const string CONFIG_VERSION = "1.0"; // Increment this whenever you change your mod's config file.

        internal static readonly FieldInfo inventoryField = typeof(GameScript).GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo craftField = typeof(GameScript).GetField("craft", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo gatherStorageField = typeof(GameScript).GetField("gatherStorage", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo modSlotField = typeof(GameScript).GetField("modSlot", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo inventoryQuestField = typeof(GameScript).GetField("inventoryQuest", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo storageField = typeof(GameScript).GetField("storage", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo curStoragePageField = typeof(GameScript).GetField("curStoragePage", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static int SelectedChallengeLevel;
        internal static int UnlockedChallengeLevel;

        private static AudioClip UseCorruptCharmSound;

        public static ItemInfo corruptCharm;
        public static TextMesh txtCorruption;

        public static Material ChallengeButton { get; private set; } = new Material(Shader.Find("Unlit/Transparent"));
        public static Material ChallengeButtonWithTrophy { get; private set; } = new Material(Shader.Find("Unlit/Transparent"));
        public static Material LeftArrow { get; private set; } = new Material(Shader.Find("Unlit/Transparent"));
        public static Material RightArrow { get; private set; } = new Material(Shader.Find("Unlit/Transparent"));
        public static Material ChallengeButton2 { get; private set; } = new Material(Shader.Find("Unlit/Transparent"));
        public static Material ChallengeButtonWithTrophy2 { get; private set; } = new Material(Shader.Find("Unlit/Transparent"));
        public static Material LeftArrow2 { get; private set; } = new Material(Shader.Find("Unlit/Transparent"));
        public static Material RightArrow2 { get; private set; } = new Material(Shader.Find("Unlit/Transparent"));

        protected override void Initialize()
        {
            Logger.Log("Challenge+ v" + Info.Mod.Version);
            SceneManager.sceneLoaded += OnSceneLoad;
            UseCorruptCharmSound = (AudioClip)Resources.Load("au/creditacquire");
            Texture corruptCharmTex = GadgetCoreAPI.LoadTexture2D("CorruptCharm.png");
            corruptCharm = new ItemInfo(ItemType.GENERIC, "Corrupt Charm", "This charm holds\na dark power...", corruptCharmTex, 1000).Register("CorruptCharm");
            corruptCharm.AddToLootTable("chest:basic", 0.1f, 1, 1, (pos) => { return GameScript.challengeLevel > 0; }, (item, pos) => { item.corrupted = GameScript.challengeLevel; return true; });
            corruptCharm.AddToLootTable("chest:gold", 0.2f, 1, 1, (pos) => { return GameScript.challengeLevel > 0; }, (item, pos) => { item.corrupted = GameScript.challengeLevel; return true; });
            UnlockedChallengeLevel = PreviewLabs.PlayerPrefs.GetInt("UnlockedChallengeLevel", 0);
            GadgetCoreAPI.RegisterCustomRPC("SetChallengeLevel", (object[] p) => {
                GameScript.challengeLevel = (int)p[0];
            });
            Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
            ChallengeButton.mainTexture = GadgetCoreAPI.LoadTexture2D("ChallengeButton.png");
            ChallengeButtonWithTrophy.mainTexture = GadgetCoreAPI.LoadTexture2D("ChallengeButtonWithTrophy.png");
            LeftArrow.mainTexture = GadgetCoreAPI.LoadTexture2D("LeftArrow.png");
            RightArrow.mainTexture = GadgetCoreAPI.LoadTexture2D("RightArrow.png");
            ChallengeButton2.mainTexture = GadgetCoreAPI.LoadTexture2D("ChallengeButton2.png");
            ChallengeButtonWithTrophy2.mainTexture = GadgetCoreAPI.LoadTexture2D("ChallengeButtonWithTrophy2.png");
            LeftArrow2.mainTexture = GadgetCoreAPI.LoadTexture2D("LeftArrow2.png");
            RightArrow2.mainTexture = GadgetCoreAPI.LoadTexture2D("RightArrow2.png");

            GadgetCoreAPI.RegisterStatModifier((Item item) =>
            {
                return EquipStatsDouble.ONE * Math.Pow(1.5, item.corrupted);
            }, StatModifierType.FinalExpMult);

            GadgetConsole.RegisterCommand("setchallengelevel", false, (sender, args) =>
            {
                if (args.Length != 2) return GadgetConsole.CommandSyntaxError(args[0], "<level>");
                if (SelectedChallengeLevel > UnlockedChallengeLevel && !GadgetConsole.IsOperator(GadgetCoreAPI.GetPlayerName())) return new GadgetConsole.GadgetConsoleMessage("You have not yet unlocked that challenge level, and you are not an operator!", null, GadgetConsole.MessageSeverity.ERROR);
                InstanceTracker.GameScript.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/confirm"), Menuu.soundLevel / 10f);
                if (int.TryParse(args[1], out int level))
                {
                    SelectedChallengeLevel = level;
                    InstanceTracker.GameScript.InvokeMethod("RefreshChallengeButton");
                    InstanceTracker.GameScript.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/purchaseDif"), Menuu.soundLevel / 10f);
                    GadgetCoreAPI.CallCustomRPC("SetChallengeLevel", RPCMode.All, SelectedChallengeLevel);
                    return new GadgetConsole.GadgetConsoleMessage("The challenge level has been set to level " + level);
                }
                else
                {
                    return new GadgetConsole.GadgetConsoleMessage("'" + args[1] + "' is not a valid number!", null, GadgetConsole.MessageSeverity.ERROR);
                }
            },
                "Sets the current challenge level.",
                "Sets the current challenge level. Enemy stats will not update until the next zone.\nUses the syntax: /setchallengelevel <level>",
                "setchallenge", "schallenge", "setcl", "setc", "scl");

            DoubleCoreAPI.RegisterPlayerDamageProcessor(ModID, PatchHelper.ApplyBonusDamage);
            DoubleCoreAPI.RegisterEnemyHealthProcessor(ModID, (hp, e) =>
            {
                if (e is Destroyer destroyer)
                {
                    return ScaleHealthForChallenge(hp, GameScript.challengeLevel, destroyer.isLava ? 2300 : 2000);
                }
                else if (e is DestroyerTrue destroyerTrue)
                {
                    return ScaleHealthForChallenge(hp, GameScript.challengeLevel, 2000);
                }
                else if (e is EnemyScript enemyScript)
                {
                    return !enemyScript.isEgg ? ScaleHealthForChallenge(hp, GameScript.challengeLevel, enemyScript.isBoss ? 4500 : 500) : hp;
                }
                else if (e is Hivemind hivemind)
                {
                    return ScaleHealthForChallenge(hp, GameScript.challengeLevel, hivemind.isCatas ? 0 : 3500);
                }
                else if (e is Millipede millipede)
                {
                    return ScaleHealthForChallenge(hp, GameScript.challengeLevel, millipede.isLava ? millipede.isMech ? 8000 : !millipede.isMykonogre && !millipede.isGlaedria ? 2300 : 0 : 2000);
                }
                else if (e is ScarabScript scarabScript)
                {
                    return ScaleHealthForChallenge(hp, GameScript.challengeLevel, scarabScript.isFell ? 0 : 3500);
                }
                else if (e is SliverScript sliverScript)
                {
                    return ScaleHealthForChallenge(hp, GameScript.challengeLevel, 150);
                }
                else if (e is WormScript wormScript)
                {
                    return ScaleHealthForChallenge(hp, GameScript.challengeLevel, 100);
                }
                else return hp;
            });
            DoubleCoreAPI.RegisterEnemyEXPProcessor(ModID, (exp) => ScaleExpForChallenge(exp, GameScript.challengeLevel));
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (!GetSingleton().Enabled) return;
            if (scene.buildIndex == 1)
            {
                if (corruptCharm != null)
                {
                    txtCorruption = Object.Instantiate(InstanceTracker.GameScript.hoverItem.transform.Find("txtName").gameObject, InstanceTracker.GameScript.hoverItem.transform).GetComponent<TextMesh>();
                    txtCorruption.gameObject.SetActive(false);
                    txtCorruption.gameObject.name = "txtCorruption";
                    txtCorruption.transform.localPosition = new Vector3(0, -1.1f, txtCorruption.transform.localPosition.z);
                    txtCorruption.anchor = TextAnchor.LowerCenter;
                    txtCorruption.alignment = TextAlignment.Center;
                }
                SelectedChallengeLevel = 0;
                Transform bSelectChallenge = InstanceTracker.GameScript.menuPlanets.transform.Find("bChallenge");
                bSelectChallenge.name = "bSelectChallenge";
                bSelectChallenge.transform.localPosition = new Vector3(0.02f, 0.4f, 0.25f);
                bSelectChallenge.GetComponent<MeshRenderer>().material = ChallengeButton;
                bSelectChallenge.GetComponent<ButtonMenu>().button = ChallengeButton;
                bSelectChallenge.GetComponent<ButtonMenu>().buttonSelect = ChallengeButton2;
                bSelectChallenge.GetComponent<BoxCollider>().center = new Vector3(-0.01f, 0f, 0f);
                bSelectChallenge.GetComponent<BoxCollider>().size = new Vector3(1.0875f, 0.1625f, 0f);
                bSelectChallenge.Find("txtPlanet").transform.localPosition = new Vector3(0.28f, 0f, 0.22f);
                bSelectChallenge.Find("txtRelics").transform.localPosition = new Vector3(-0.5f, 0f, 0.25f);
                Transform bLowerChallenge = Object.Instantiate(bSelectChallenge.gameObject).GetComponent<Transform>();
                bLowerChallenge.name = "bLowerChallenge";
                bLowerChallenge.SetParent(bSelectChallenge.parent, false);
                bLowerChallenge.gameObject.tag = "Untagged";
                bLowerChallenge.localPosition = new Vector3(-0.3f, 0.4f, 0.25f);
                bLowerChallenge.GetComponent<MeshRenderer>().material = LeftArrow;
                bLowerChallenge.GetComponent<ButtonMenu>().button = LeftArrow;
                bLowerChallenge.GetComponent<ButtonMenu>().buttonSelect = LeftArrow2;
                bLowerChallenge.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, 0f);
                bLowerChallenge.GetComponent<BoxCollider>().size = new Vector3(0.1625f, 0.1625f, 0f);
                foreach (Transform child in bLowerChallenge) Object.Destroy(child.gameObject);
                Transform bRaiseChallenge = Object.Instantiate(bSelectChallenge.gameObject).GetComponent<Transform>();
                bRaiseChallenge.name = "bRaiseChallenge";
                bRaiseChallenge.SetParent(bSelectChallenge.parent, false);
                bRaiseChallenge.gameObject.tag = "Untagged";
                bRaiseChallenge.localPosition = new Vector3(0.333f, 0.4f, 0.25f);
                bRaiseChallenge.GetComponent<MeshRenderer>().material = RightArrow;
                bRaiseChallenge.GetComponent<ButtonMenu>().button = RightArrow;
                bRaiseChallenge.GetComponent<ButtonMenu>().buttonSelect = RightArrow2;
                bRaiseChallenge.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, 0f);
                bRaiseChallenge.GetComponent<BoxCollider>().size = new Vector3(0.1625f, 0.1625f, 0f);
                foreach (Transform child in bRaiseChallenge) Object.Destroy(child.gameObject);
            }
        }

        internal static bool UseCharm(ref Item charm, int slot)
        {
            Item[] inventory = (Item[])inventoryField.GetValue(InstanceTracker.GameScript);
            if ((ItemRegistry.GetItem(inventory[slot].id).Type & ItemType.LEVELING) != ItemType.LEVELING) return false;
            if (InstanceTracker.GameScript.GetItemLevel(inventory[slot].exp) >= 10)
            {
                if (charm.corrupted > inventory[slot].corrupted)
                {
                    inventory[slot].corrupted++;
                    inventory[slot].exp = 0;
                    InstanceTracker.GameScript.GetComponent<AudioSource>().PlayOneShot(UseCorruptCharmSound, Menuu.soundLevel / 10f);
                    charm.q--;
                    if (charm.q <= 0) charm = GadgetCoreAPI.EmptyItem();
                    int[] gearStats = GameScript.GEARSTAT.ToArray();
                    InstanceTracker.GameScript.InvokeMethod("RefreshSlot", slot);
                    InstanceTracker.GameScript.InvokeMethod("RefreshHoldingSlot");
                    InstanceTracker.GameScript.InvokeMethod("RefreshStats");
                    GadgetCore.PatchMethods.RecalculateGearStats(inventory);
                    for (int i = 0;i < gearStats.Length;i++)
                    {
                        if (gearStats[i] != GameScript.GEARSTAT[i]) InstanceTracker.GameScript.txtPlayerStat[i].GetComponent<Animation>().Play();
                    }
                }
                else
                {
                    GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("txtError"), GadgetCoreAPI.GetCursorPos(), Quaternion.identity);
                    gameObject.SendMessage("InitError", "Unable to apply the charm to your gear because it isn't stronger than the gear!");
                }
            }
            else
            {
                GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("txtError"), GadgetCoreAPI.GetCursorPos(), Quaternion.identity);
                gameObject.SendMessage("InitError", "Unable to apply the charm to your gear because the gear is not max level!");
            }
            return true;
        }

        public static int ConvertDoubleToInt(double value)
        {
            if (value >= int.MaxValue)
                return int.MaxValue;
            else if (value <= int.MinValue)
                return int.MinValue;
            else
                return (int)value;
        }

        public static double ScaleHealthForChallenge(double health, int challenge, int vanillaBonus = 0)
        {
            if (challenge < 1) return health;
            return (health - ((challenge - 1) * vanillaBonus)) * Math.Pow(2.125, challenge - 1);
        }

        public static int ScaleDamageForChallenge(int damage, int challenge, int vanillaBonus = 0)
        {
            if (challenge < 1) return damage;
            return damage - ((challenge - 1) * vanillaBonus);
        }

        public static double ScaleDamageMultForChallenge(int challenge)
        {
            if (challenge < 1) return 1;
            return Math.Pow(1.25, challenge - 1);
        }

        public static double ScaleExpForChallenge(double exp, int challenge, int vanillaBonus = 0)
        {
            if (challenge < 1) return exp;
            return (exp - ((challenge - 1) * vanillaBonus)) * Math.Pow(1.5, challenge - 1);
        }

        public static int ScaleGearExpForChallenge(int exp, int challenge)
        {
            if (challenge < 1) return exp;
            return ConvertDoubleToInt(exp * Math.Pow(1.25, challenge) * 4);
        }

        public static int ScaleCreditsForChallenge(int credits, int challenge)
        {
            if (challenge < 1) return credits;
            return credits * challenge;
        }

        public static int ScaleResourcesForChallenge(int resources, int challenge)
        {
            if (challenge < 1) return resources;
            double unroundedResources = resources * Math.Pow(1.125, challenge);
            if (unroundedResources % 1 > Random.value)
            {
                return (int)Math.Ceiling(unroundedResources);
            }
            else
            {
                return (int)Math.Floor(unroundedResources);
            }
        }

        public static double GetBonusDamageMult(Item item)
        {
            int challenge = item.corrupted;
            if (challenge < 1) return 1;
            EquipStats stats = GadgetCoreAPI.GetGearStats(item);
            int totalStats = stats.VIT + stats.STR + stats.DEX + stats.TEC + stats.MAG + stats.FTH;
            if (totalStats == 0) return 1;
            int targetStat;
            if (item.id >= 300 && item.id < 400)
                targetStat = stats.STR;
            else if (item.id >= 400 && item.id < 500)
                targetStat = stats.DEX;
            else if (item.id >= 500 && item.id < 550)
                targetStat = stats.MAG;
            else if (item.id >= 550 && item.id < 600)
                targetStat = stats.FTH;
            else
            {
                ItemInfo itemInfo = ItemRegistry.GetItem(item.id);
                targetStat = stats.GetByIndex(itemInfo.WeaponScaling.ToList().IndexOf(itemInfo.WeaponScaling.Max()));
            }
            double statProportion = targetStat / (double)totalStats;
            return Math.Pow(1 + (0.25 * statProportion), challenge);
        }

        public static double GetBonusDeflectChance(Item item)
        {
            int challenge = item.corrupted;
            if (challenge < 1) return 0;
            EquipStats stats = GadgetCoreAPI.GetGearStats(item);
            int totalStats = stats.VIT + stats.STR + stats.DEX + stats.TEC + stats.MAG + stats.FTH;
            if (totalStats == 0) return 0;
            int targetStat = stats.DEX;
            double statProportion = targetStat / (double)totalStats;
            return 1 - Math.Pow(0.875 - (statProportion / 4), challenge);
        }

        public static double GetBonusDamageReduction(Item item)
        {
            int challenge = item.corrupted;
            if (challenge < 1) return 0;
            EquipStats stats = GadgetCoreAPI.GetGearStats(item);
            int totalStats = stats.VIT + stats.STR + stats.DEX + stats.TEC + stats.MAG + stats.FTH;
            if (totalStats == 0) return 0;
            int targetStat = stats.VIT;
            double statProportion = targetStat / (double)totalStats;
            return 0.5 - (Math.Pow(0.875 - (statProportion / 4), challenge) * 0.5);
        }

        public static double GetBonusRareDropChance(int challenge)
        {
            return 0.5 * challenge;
        }

        public static double GetBonusMiningSpeedMult(int challenge)
        {
            return Math.Pow(1.125, challenge);
        }
    }
}