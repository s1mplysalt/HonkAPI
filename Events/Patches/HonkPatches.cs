using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using HonkAPI.Events;
using HonkAPI.Props;
using HonkAPI.UI;

namespace HonkAPI.Events.Patches
{
    [HarmonyPatch(typeof(Goose), "Start")]
    internal static class GooseStartPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Goose __instance)
        {
            HonkEvents.InvokeGooseSpawn(__instance);
        }
    }

    [HarmonyPatch(typeof(GooseHonker), "PlayHonkSound")]
    internal static class HonkSoundPatch
    {
        [HarmonyPostfix]
        private static void Postfix(GooseHonker __instance, Goose ___goose)
        {
            HonkEvents.InvokeHonk(___goose, false);
        }
    }

    [HarmonyPatch(typeof(GooseHonker), "PlayGlobalHonk")]
    internal static class GlobalHonkPatch
    {
        [HarmonyPostfix]
        private static void Postfix(GooseHonker __instance, Goose ___goose)
        {
            HonkEvents.InvokeHonk(___goose, true);
        }
    }

    [HarmonyPatch(typeof(Prop), "Awake")]
    internal static class PropAwakePatch
    {
        [HarmonyPrefix]
        private static void Prefix(Prop __instance)
        {
            if (__instance.holdSiblings == null)
            {
                __instance.holdSiblings = new Rigidbody[0];
            }
            if (__instance.annoyingSwitches == null)
            {
                __instance.annoyingSwitches = new SwitchReference[0];
            }
            if (__instance.extraOwners == null)
            {
                __instance.extraOwners = new List<Folk>();
            }
            if (__instance.nonCleaningOwners == null)
            {
                __instance.nonCleaningOwners = new List<Folk>();
            }
            if (__instance.secondaryPropGroups == null)
            {
                __instance.secondaryPropGroups = new List<PropGroup>();
            }
            if (__instance.propXray == null)
            {
                __instance.propXray = new PropXray();
                __instance.propXray.dontShowXray = true;
            }
        }
    }

    [HarmonyPatch(typeof(Prop), "SafelySnap")]
    internal static class PropSafelySnapPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(PropHome home)
        {
            return home != null;
        }
    }

    [HarmonyPatch(typeof(Prop), "Update")]
    internal static class PropUpdatePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Prop __instance, ref CameraManager ___cameraManager)
        {
            if (__instance == null || !__instance.isActiveAndEnabled)
            {
                return false;
            }
            if (___cameraManager == null)
            {
                if (GameManager.instance != null && GameManager.instance.cameraManager != null)
                {
                    ___cameraManager = GameManager.instance.cameraManager;
                }
                else
                {
                    return false;
                }
            }
            if (__instance.propXray == null)
            {
                __instance.propXray = new PropXray();
                __instance.propXray.dontShowXray = true;
            }
            if (__instance.propNMOController == null)
            {
                __instance.propNMOController = new PropNMOController(__instance);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Prop), "ResetPosition")]
    internal static class PropResetPositionPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Prop __instance)
        {
            if (__instance == null || __instance.startHome == null)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PropXray), "Update")]
    internal static class PropXrayUpdatePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(PropXray __instance)
        {
            if (__instance == null || __instance.prop == null || __instance.sharedData == null)
            {
                return false;
            }
            if (__instance.cameraManager == null || __instance.myCamera == null)
            {
                if (GameManager.instance != null && GameManager.instance.cameraManager != null)
                {
                    __instance.cameraManager = GameManager.instance.cameraManager;
                    __instance.myCamera = GameManager.instance.cameraManager.myCamera;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PropXray), "Initialize")]
    internal static class PropXrayInitializePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(PropXray __instance, Prop prop)
        {
            if (GameManager.instance == null || GameManager.instance.propXraySharedData == null || GameManager.instance.cameraManager == null)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(StandaloneSaveSystem), "LoadSlot")]
    internal static class StandaloneSaveSystemLoadSlotPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(StandaloneSaveSystem __instance, int slot, ref SaveGameData __result)
        {
            string savePath = __instance.PersistentDataPath + "/savegame" + slot + ".save";
            if (!System.IO.File.Exists(savePath))
            {
                __result = new SaveGameData();
                return false;
            }
            try
            {
                using (System.IO.FileStream stream = System.IO.File.Open(savePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bf.Binder = new StandaloneSaveSystem.SaveGameDataBinder();
                    __result = (SaveGameData)bf.Deserialize(stream);
                }
                if (__result == null)
                {
                    __result = new SaveGameData();
                }
                return false;
            }
            catch (Exception)
            {
                __result = new SaveGameData();
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(GameManager), "SaveTimePlayed")]
    internal static class GameManagerSaveTimePlayedPatch
    {
        [HarmonyPrefix]
        private static bool Prefix()
        {
            if (GameManager.instance == null)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Loading), "UnloadGame")]
    internal static class LoadingUnloadGamePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Action callback)
        {
            BaseManager.gameWorldActive = false;
            int sceneCount = SceneManager.sceneCount;
            Loading.unloadCallback = callback;

            if (sceneCount <= 1)
            {
                if (Loading.unloadCallback != null)
                {
                    Action cb = Loading.unloadCallback;
                    Loading.unloadCallback = null;
                    cb();
                }
                return false;
            }

            for (int i = 1; i < sceneCount; i++)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i)).completed += Loading.CheckIfUnloadingFinished;
            }
            Time.timeScale = 1f;
            Resources.UnloadUnusedAssets();
            return false;
        }
    }

    [HarmonyPatch(typeof(Loading), "CheckIfUnloadingFinished")]
    internal static class LoadingCheckIfUnloadingFinishedPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(AsyncOperation ao)
        {
            if (SceneManager.sceneCount <= 1 && Loading.unloadCallback != null)
            {
                Action cb = Loading.unloadCallback;
                Loading.unloadCallback = null;
                cb();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(BeakHandle), "Start")]
    internal static class BeakHandleStartPatch
    {
        [HarmonyPrefix]
        private static void Prefix(BeakHandle __instance)
        {
            if (__instance.tugOfWarJob == null)
            {
                __instance.tugOfWarJob = new JobReference();
            }
        }
    }

    [HarmonyPatch(typeof(Prop), "PickUp")]
    internal static class PropPickUpPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Prop __instance)
        {
            if (__instance.heldBy != null)
            {
                Goose g = __instance.heldBy.GetComponent<Goose>();
                if (g != null)
                {
                    HonkEvents.InvokeGrabProp(g, __instance);
                    HonkPropCallbacks cb = __instance.GetComponent<HonkPropCallbacks>();
                    if (cb != null && cb.onGrabbed != null)
                    {
                        cb.onGrabbed(g);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Prop), "PutDown", new Type[] { })]
    internal static class PropPutDownPatch
    {
        [HarmonyPrefix]
        private static void Prefix(Prop __instance)
        {
            if (__instance.heldBy != null)
            {
                Goose g = __instance.heldBy.GetComponent<Goose>();
                if (g != null)
                {
                    HonkEvents.InvokeDropProp(g, __instance);
                    HonkPropCallbacks cb = __instance.GetComponent<HonkPropCallbacks>();
                    if (cb != null && cb.onDropped != null)
                    {
                        cb.onDropped(g);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(ScaredState), "Enter")]
    internal static class ScaredStateEnterPatch
    {
        [HarmonyPostfix]
        private static void Postfix(ScaredState __instance)
        {
            if (__instance != null && __instance.brain != null)
            {
                HonkEvents.InvokeNPCScared(__instance.brain);
            }
        }
    }

    [HarmonyPatch(typeof(GoalManager), "OnAward")]
    internal static class GoalManagerAwardPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Goal goal)
        {
            if (goal != null)
            {
                HonkEvents.InvokeGoalAward(goal);
            }
        }
    }

    [HarmonyPatch(typeof(MainMenu), "Initialize")]
    internal static class MainMenuInitializePatch
    {
        [HarmonyPostfix]
        private static void Postfix(MainMenu __instance)
        {
            HonkMainMenuUI.SetupModsMenu(__instance);
        }
    }

    [HarmonyPatch(typeof(MainMenu), "OnEnable")]
    internal static class MainMenuOnEnablePatch
    {
        [HarmonyPostfix]
        private static void Postfix(MainMenu __instance)
        {
            HonkMainMenuUI.EnsureModsMenu(__instance);
        }
    }
}
