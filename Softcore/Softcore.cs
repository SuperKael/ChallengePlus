using UnityEngine;
using UModFramework.API;

namespace Softcore
{
    class Softcore
    {
        internal static void Log(string text, bool clean = false)
        {
            using (UMFLog log = new UMFLog()) log.Log(text, clean);
        }

        [UMFConfig]
        public static void LoadConfig()
        {
            SoftcoreConfig.Load();
        }

		[UMFHarmony(2)] //Set this to the number of harmony patches in your mod.
        public static void Start()
		{
			Log("Softcore v" + UMFMod.GetModVersion().ToString(), true);
		}
	}
}