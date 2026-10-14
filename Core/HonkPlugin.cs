using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HonkAPI.Core;
using HonkAPI.Events;

namespace HonkAPI
{
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
    public class HonkPlugin : BaseUnityPlugin
    {
        public const string MOD_GUID = "com.honkapi.core";
        public const string MOD_NAME = "HonkAPI";
        public const string MOD_VERSION = "2.0.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo(string.Format("{0} v{1} initialized.", MOD_NAME, MOD_VERSION));

            Harmony harmony = new Harmony(MOD_GUID);
            harmony.PatchAll();

            HonkModLoader.Initialize(Logger);
        }

        private bool areaTracked;
        private GoalListArea lastArea;

        private void Update()
        {
            if (BaseManager.gameWorldActive)
            {
                GoalListArea area = AreaTracker.mostRecentArea;
                if (areaTracked && area != lastArea)
                {
                    HonkEvents.InvokeAreaChange(area);
                }
                lastArea = area;
                areaTracked = true;
            }
            else
            {
                areaTracked = false;
            }

            HonkEvents.InvokeUpdate();
        }

        private void OnGUI()
        {
            HonkEvents.InvokeGUI();
        }
    }
}
