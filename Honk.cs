using System;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using HonkAPI.Audio;
using HonkAPI.CameraSystem;
using HonkAPI.Cheats;
using HonkAPI.Debugging;
using HonkAPI.Events;
using HonkAPI.Goals;
using HonkAPI.GooseApi;
using HonkAPI.InputSystem;
using HonkAPI.NPC;
using HonkAPI.PhysicsApi;
using HonkAPI.Props;
using HonkAPI.UI;
using HonkAPI.Weather;
using HonkAPI.World;

namespace HonkAPI
{
    public static partial class Honk
    {
        public static partial class Goose
        {
            public static global::Goose Current
            {
                get { return GooseAPI.Current; }
            }

            public static List<global::Goose> AllGeese
            {
                get { return GooseAPI.AllGeese; }
            }

            public static void HonkSound(float pitch = 1f)
            {
                GooseAPI.Honk(pitch);
            }

            public static void FlapWings()
            {
                GooseAPI.FlapWings();
            }

            public static void DuckHead(float amount = 1f)
            {
                GooseAPI.DuckHead(amount);
            }

            public static void Teleport(Vector3 position)
            {
                GooseAPI.Teleport(position);
            }

            public static void Teleport(float x, float y, float z)
            {
                GooseAPI.Teleport(x, y, z);
            }

            public static void SetScale(float scale)
            {
                GooseAPI.SetScale(scale);
            }

            public static void ResetScale()
            {
                GooseAPI.ResetScale();
            }

            public static void SetSpeedMultiplier(float mult)
            {
                GooseAPI.SetSpeedMultiplier(mult);
            }

            public static void ResetSpeed()
            {
                GooseAPI.ResetSpeed();
            }

            public static bool CycleTeleportPoint(int delta = 1)
            {
                return GooseAPI.CycleTeleportPoint(delta);
            }

            public static bool TeleportToArea(GoalListArea area)
            {
                return GooseAPI.TeleportToArea(area);
            }

            public static global::Prop GetHeldProp()
            {
                return GooseAPI.GetHeldProp();
            }

            public static void DropHeldProp()
            {
                GooseAPI.DropHeldProp();
            }

            public static void GrabProp(global::Prop prop)
            {
                GooseAPI.GrabProp(prop);
            }
        }

        public static class NPCs
        {
            public static List<global::Brain> GetAll()
            {
                return NPCAPI.GetAll();
            }

            public static global::Brain GetClosest(Vector3 position)
            {
                return NPCAPI.GetClosest(position);
            }

            public static void Scare(global::Brain npc)
            {
                NPCAPI.Scare(npc);
            }

            public static void ScareAll(float radius = 50f)
            {
                NPCAPI.ScareAll(radius);
            }

            public static void Calm(global::Brain npc)
            {
                NPCAPI.Calm(npc);
            }

            public static void CalmAll()
            {
                NPCAPI.CalmAll();
            }

            public static void ClearPunctuation(global::Brain npc)
            {
                NPCAPI.ClearPunctuation(npc);
            }

            public static void Distract(global::Brain npc, Vector3 lookAtPosition)
            {
                NPCAPI.Distract(npc, lookAtPosition);
            }

            public static void MakeDropHeld(global::Brain npc)
            {
                NPCAPI.MakeDropHeld(npc);
            }

            public static void MakeGrumpy(global::Brain npc)
            {
                NPCAPI.MakeGrumpy(npc);
            }

            public static void Teleport(global::Brain npc, Vector3 position)
            {
                NPCAPI.Teleport(npc, position);
            }

            public static void SetWalkSpeed(global::Brain npc, float speed)
            {
                NPCAPI.SetWalkSpeed(npc, speed);
            }

            public static void ResetSpeed(global::Brain npc)
            {
                NPCAPI.ResetSpeed(npc);
            }

            public static void ResetAllSpeeds()
            {
                NPCAPI.ResetAllSpeeds();
            }

            public static void Alert(global::Brain npc)
            {
                NPCAPI.Alert(npc);
            }

            public static void Confuse(global::Brain npc)
            {
                NPCAPI.Confuse(npc);
            }

            public static void WalkTo(global::Brain npc, Vector3 destination)
            {
                NPCAPI.WalkTo(npc, destination);
            }

            /// <summary>All 62 brain states: query, request, force, suppress.</summary>
            public static class States
            {
                public static event BrainStateChangedHandler OnChanged
                {
                    add { BrainStateAPI.OnStateChanged += value; }
                    remove { BrainStateAPI.OnStateChanged -= value; }
                }

                public static HumanState? Current(global::Brain npc) { return BrainStateAPI.GetCurrent(npc); }
                public static bool IsIn(global::Brain npc, HumanState s) { return BrainStateAPI.IsIn(npc, s); }
                public static bool IsDesired(global::Brain npc, HumanState s) { return BrainStateAPI.IsDesired(npc, s); }
                public static List<HumanState> Desired(global::Brain npc) { return BrainStateAPI.GetDesired(npc); }
                public static string Describe(global::Brain npc) { return BrainStateAPI.Describe(npc); }
                public static IHumanState Get(global::Brain npc, HumanState s) { return BrainStateAPI.GetState(npc, s); }
                public static T Get<T>(global::Brain npc, HumanState s) where T : class { return BrainStateAPI.GetState<T>(npc, s); }
                public static ReactionState Reaction(global::Brain npc, ReactionName r) { return BrainStateAPI.GetReaction(npc, r); }

                public static void Request(global::Brain npc, HumanState s) { BrainStateAPI.Request(npc, s); }
                public static void CancelRequest(global::Brain npc, HumanState s) { BrainStateAPI.CancelRequest(npc, s); }
                public static void ClearRequests(global::Brain npc) { BrainStateAPI.ClearAllRequests(npc); }
                public static ReactionState Trigger(global::Brain npc, ReactionName r) { return BrainStateAPI.TriggerReaction(npc, r); }

                public static bool Force(global::Brain npc, HumanState s, float duration = 0f) { return BrainStateAPI.Force(npc, s, duration); }
                public static void Release(global::Brain npc) { BrainStateAPI.Release(npc); }
                public static void ReleaseAll() { BrainStateAPI.ReleaseAll(); }
                public static bool IsForced(global::Brain npc) { return BrainStateAPI.IsForced(npc); }

                public static void Suppress(global::Brain npc, HumanState s) { BrainStateAPI.Suppress(npc, s); }
                public static void Unsuppress(global::Brain npc, HumanState s) { BrainStateAPI.Unsuppress(npc, s); }
                public static void SuppressEverywhere(HumanState s) { BrainStateAPI.SuppressEverywhere(s); }
                public static void UnsuppressEverywhere(HumanState s) { BrainStateAPI.UnsuppressEverywhere(s); }
                public static void ClearSuppression() { BrainStateAPI.ClearSuppression(); }
            }

            /// <summary>Targets, speeds, permissions, goose awareness and reaction tuning.</summary>
            public static class Control
            {
                public static Brain FindByFolk(Folk who) { return BrainControlAPI.FindByFolk(who); }
                public static void SetTarget(global::Brain npc, GameObject target) { BrainControlAPI.SetTarget(npc, target); }
                public static void SetLookTarget(global::Brain npc, GameObject target) { BrainControlAPI.SetLookTarget(npc, target); }
                public static void FaceTowards(global::Brain npc, Vector3 p) { BrainControlAPI.FaceTowards(npc, p); }
                public static void Stop(global::Brain npc) { BrainControlAPI.Stop(npc); }
                public static void SetUrgent(global::Brain npc, bool u) { BrainControlAPI.SetUrgent(npc, u); }
                public static void SetCasualSpeed(global::Brain npc, float s) { BrainControlAPI.SetCasualSpeed(npc, s); }
                public static void SetHurriedSpeed(global::Brain npc, float s) { BrainControlAPI.SetHurriedSpeed(npc, s); }
                public static bool CanSeeGoose(global::Brain npc) { return BrainControlAPI.CanSeeGoose(npc); }
                public static bool LostTrackOfGoose(global::Brain npc) { return BrainControlAPI.LostTrackOfGoose(npc); }
                public static Vector3 AssumedGoosePosition(global::Brain npc) { return BrainControlAPI.GetAssumedGoosePosition(npc); }
                public static void SetAssumedGoosePosition(global::Brain npc, Vector3 p) { BrainControlAPI.SetAssumedGoosePosition(npc, p); }
                public static void ForgetGoose(global::Brain npc) { BrainControlAPI.ForgetGoose(npc); }
                public static void SetAllowTruckJobs(global::Brain npc, bool v) { BrainControlAPI.SetAllowTruckJobs(npc, v); }
                public static void SetAllowVisiting(global::Brain npc, bool v) { BrainControlAPI.SetAllowVisiting(npc, v); }
                public static void SetAllowPropJobs(global::Brain npc, bool v) { BrainControlAPI.SetAllowPropJobs(npc, v); }
                public static ReactionState ConfigureReaction(global::Brain npc, ReactionName name, BrainControlAPI.ReactionOptions o, bool trigger) { return BrainControlAPI.ConfigureReaction(npc, name, o, trigger); }
            }
        }

        public static partial class Props
        {
            public static PropBuilder Create(string name)
            {
                return PropAPI.Create(name);
            }

            public static List<global::Prop> GetAll()
            {
                return PropAPI.GetAll();
            }

            public static global::Prop GetClosest(Vector3 position)
            {
                return PropAPI.GetClosest(position);
            }

            public static global::Prop FindByName(string name)
            {
                return PropAPI.FindByName(name);
            }

            public static void Launch(global::Prop prop, Vector3 force)
            {
                PropAPI.Launch(prop, force);
            }

            public static void Teleport(global::Prop prop, Vector3 position)
            {
                PropAPI.Teleport(prop, position);
            }

            public static void Destroy(global::Prop prop)
            {
                PropAPI.Destroy(prop);
            }
        }

        public static partial class Physics
        {
            public static void Explosion(Vector3 origin, float force, float radius, float upwardModifier = 1f)
            {
                PhysicsAPI.Explosion(origin, force, radius, upwardModifier);
            }

            public static void Attract(Vector3 center, float force, float radius)
            {
                PhysicsAPI.Attract(center, force, radius);
            }

            public static void SetGravity(Vector3 gravity)
            {
                PhysicsAPI.SetGravity(gravity);
            }

            public static void SetGravity(float x, float y, float z)
            {
                PhysicsAPI.SetGravity(x, y, z);
            }

            public static void ResetGravity()
            {
                PhysicsAPI.ResetGravity();
            }

            public static void ZeroGravity()
            {
                PhysicsAPI.ZeroGravity();
            }

            public static void LaunchAllProps(Vector3 force)
            {
                PhysicsAPI.LaunchAllProps(force);
            }

            public static void FreezeAllProps(bool freeze)
            {
                PhysicsAPI.FreezeAllProps(freeze);
            }
        }

        public static partial class Cheats
        {
            public static void SetAllowCheats(bool allow)
            {
                CheatsAPI.SetAllowCheats(allow);
            }

            public static void CompleteAllGoals()
            {
                CheatsAPI.CompleteAllGoals();
            }

            public static void TeleportToArea(global::GoalListArea area)
            {
                CheatsAPI.TeleportToArea(area);
            }

            public static void SetSuperSpeed(bool enable, float mult = 2.5f)
            {
                CheatsAPI.SetSuperSpeed(enable, mult);
            }

            public static void SetGiantGoose(bool enable, float scale = 2.5f)
            {
                CheatsAPI.SetGiantGoose(enable, scale);
            }

            public static void SetTinyGoose(bool enable, float scale = 0.35f)
            {
                CheatsAPI.SetTinyGoose(enable, scale);
            }
        }

        public static partial class Input
        {
            public static bool GetKeyDown(KeyCode key)
            {
                return InputAPI.GetKeyDown(key);
            }

            public static bool GetKey(KeyCode key)
            {
                return InputAPI.GetKey(key);
            }

            public static bool GetKeyUp(KeyCode key)
            {
                return InputAPI.GetKeyUp(key);
            }

            public static bool IsHonkPressed()
            {
                return InputAPI.IsHonkPressed();
            }

            public static bool IsSprintPressed()
            {
                return InputAPI.IsSprintPressed();
            }

            public static bool IsDuckPressed()
            {
                return InputAPI.IsDuckPressed();
            }

            public static void RegisterHotkey(KeyCode key, Action callback)
            {
                InputAPI.RegisterHotkey(key, callback);
            }

            public static void UnregisterHotkey(KeyCode key)
            {
                InputAPI.UnregisterHotkey(key);
            }
        }

        public static partial class Weather
        {
            public static void SetAmbientLight(Color color)
            {
                WeatherAPI.SetAmbientLight(color);
            }

            public static void SetFog(bool enabled, Color color, float density)
            {
                WeatherAPI.SetFog(enabled, color, density);
            }

            public static void SetNightMode()
            {
                WeatherAPI.SetNightMode();
            }

            public static void ResetAtmosphere()
            {
                WeatherAPI.ResetAtmosphere();
            }
        }

        public static partial class Camera
        {
            public static UnityEngine.Camera Current
            {
                get { return CameraAPI.Current; }
            }

            public static void SetFieldOfView(float fov)
            {
                CameraAPI.SetFieldOfView(fov);
            }

            public static void ResetFieldOfView()
            {
                CameraAPI.ResetFieldOfView();
            }
        }

        public static partial class Audio
        {
            public static void PlayOneShot(string soundPath, Vector3 position)
            {
                HonkAudioAPI.PlayOneShot(soundPath, position);
            }

            public static void PlayHonk(Vector3 position, float pitch = 1f)
            {
                HonkAudioAPI.PlayHonk(position, pitch);
            }

            public static void MuteMusic(bool mute)
            {
                HonkAudioAPI.MuteMusic(mute);
            }

            public static void SetMasterVolume(float volume)
            {
                HonkAudioAPI.SetMasterVolume(volume);
            }
        }

        public static partial class World
        {
            public static bool IsGameActive
            {
                get { return WorldAPI.IsGameActive; }
            }

            public static string CurrentArea
            {
                get { return WorldAPI.CurrentArea; }
            }

            public static float TimePlayed
            {
                get { return WorldAPI.TimePlayed; }
            }

            public static UnityEngine.Camera MainCamera
            {
                get { return WorldAPI.MainCamera; }
            }

            public static void SetCameraDistance(float distance)
            {
                WorldAPI.SetCameraDistance(distance);
            }

            public static void SetCameraTarget(Transform target)
            {
                WorldAPI.SetCameraTarget(target);
            }

            public static void ResetCamera()
            {
                WorldAPI.ResetCamera();
            }

            public static void SetSlowMotion(float timeScale)
            {
                WorldAPI.SetSlowMotion(timeScale);
            }

            public static void ResetTimeScale()
            {
                WorldAPI.ResetTimeScale();
            }

            public static void Pause(bool pause)
            {
                WorldAPI.PauseGame(pause);
            }
        }

        public static class UI
        {
            public static bool CinematicMode
            {
                get { return HonkUI.CinematicMode; }
                set { HonkUI.CinematicMode = value; }
            }

            public static HonkWindow CreateWindow(string title, Rect initialRect)
            {
                return HonkUI.CreateWindow(title, initialRect);
            }

            public static void RemoveWindow(HonkWindow window)
            {
                HonkUI.RemoveWindow(window);
            }

            public static void ShowNotification(string message, float duration = 3.5f)
            {
                HonkUI.ShowNotification(message, NotificationType.Info, duration);
            }

            public static void ShowSuccess(string message, float duration = 3.5f)
            {
                HonkUI.ShowSuccess(message, duration);
            }

            public static void ShowWarning(string message, float duration = 3.5f)
            {
                HonkUI.ShowWarning(message, duration);
            }

            public static void ShowError(string message, float duration = 3.5f)
            {
                HonkUI.ShowError(message, duration);
            }

            public static void ShowHonk(string message, float duration = 3.5f)
            {
                HonkUI.ShowHonk(message, duration);
            }

            public static void ShowConfirm(string title, string message, Action<bool> callback, string ok = "Confirm", string cancel = "Cancel")
            {
                HonkUI.ShowConfirm(title, message, callback, ok, cancel);
            }

            public static void ShowInput(string title, string prompt, Action<string> callback, string defaultValue = "")
            {
                HonkUI.ShowInput(title, prompt, callback, defaultValue);
            }

            public static void ToggleConsole()
            {
                HonkUI.ToggleConsole();
            }

            public static void ShowHonkScreen(bool show)
            {
                HonkUI.ShowHonkScreen(show);
            }

            public static void OpenGoalsNotebook(bool open)
            {
                HonkUI.OpenGoalsNotebook(open);
            }

            public static void OpenPauseMenu(bool open)
            {
                HonkUI.OpenPauseMenu(open);
            }

            public static class HUD
            {
                public static void AddLabel(string id, Func<string> textGetter, TextAnchor anchor = TextAnchor.UpperLeft)
                {
                    HonkHUD.AddLabel(id, textGetter, anchor);
                }

                public static void RemoveLabel(string id)
                {
                    HonkHUD.RemoveLabel(id);
                }

                public static void SetShowFPS(bool show)
                {
                    HonkHUD.SetShowFPS(show);
                }

                public static void SetShowCoordinates(bool show)
                {
                    HonkHUD.SetShowCoordinates(show);
                }

                public static void SetShowSpeedometer(bool show)
                {
                    HonkHUD.SetShowSpeedometer(show);
                }

                public static void SetShowCrosshair(bool show)
                {
                    HonkHUD.SetShowCrosshair(show);
                }

                public static void Clear()
                {
                    HonkHUD.Clear();
                }
            }
        }

        public static partial class Goals
        {
            public static List<global::Goal> GetAll()
            {
                return GoalRegistry.GetAll();
            }

            public static global::Goal Get(string goalName)
            {
                return GoalRegistry.Get(goalName);
            }

            public static void Complete(string goalName)
            {
                GoalRegistry.CompleteGoal(goalName);
            }

            public static bool IsCompleted(string goalName)
            {
                return GoalRegistry.IsCompleted(goalName);
            }

            public static void AddCustom(string goalName, string descriptionKey, global::GoalListArea area)
            {
                GoalRegistry.AddCustomGoal(goalName, descriptionKey, area);
            }

            public static void UnlockExtraPages()
            {
                GoalRegistry.UnlockExtraPages();
            }
        }

        public static partial class Events
        {
            public static event HonkEvents.GooseSpawnHandler OnGooseSpawn
            {
                add { HonkEvents.OnGooseSpawn += value; }
                remove { HonkEvents.OnGooseSpawn -= value; }
            }

            public static event HonkEvents.HonkHandler OnHonk
            {
                add { HonkEvents.OnHonk += value; }
                remove { HonkEvents.OnHonk -= value; }
            }

            public static event HonkEvents.PropGrabHandler OnGrabProp
            {
                add { HonkEvents.OnGrabProp += value; }
                remove { HonkEvents.OnGrabProp -= value; }
            }

            public static event HonkEvents.PropDropHandler OnDropProp
            {
                add { HonkEvents.OnDropProp += value; }
                remove { HonkEvents.OnDropProp -= value; }
            }

            public static event HonkEvents.NPCScaredHandler OnNPCScared
            {
                add { HonkEvents.OnNPCScared += value; }
                remove { HonkEvents.OnNPCScared -= value; }
            }

            public static event HonkEvents.GoalAwardHandler OnGoalAward
            {
                add { HonkEvents.OnGoalAward += value; }
                remove { HonkEvents.OnGoalAward -= value; }
            }

            public static event HonkEvents.TickHandler OnUpdate
            {
                add { HonkEvents.OnUpdate += value; }
                remove { HonkEvents.OnUpdate -= value; }
            }

            public static event HonkEvents.TickHandler OnGUI
            {
                add { HonkEvents.OnGUI += value; }
                remove { HonkEvents.OnGUI -= value; }
            }
        }

        public static class Debug
        {
            public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 1.0f)
            {
                HonkDebugAPI.DrawLine(start, end, color, duration);
            }

            public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 1.0f)
            {
                HonkDebugAPI.DrawRay(start, dir, color, duration);
            }

            public static void Log(string message)
            {
                HonkDebugAPI.Log(message);
            }

            public static void LogWarning(string message)
            {
                HonkDebugAPI.LogWarning(message);
            }

            public static void LogError(string message)
            {
                HonkDebugAPI.LogError(message);
            }
        }
    }
}
