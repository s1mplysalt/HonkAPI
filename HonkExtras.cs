using System;
using System.Collections.Generic;
using UnityEngine;
using HonkAPI.Audio;
using HonkAPI.CameraSystem;
using HonkAPI.Cheats;
using HonkAPI.Events;
using HonkAPI.Goals;
using HonkAPI.GooseApi;
using HonkAPI.InputSystem;
using HonkAPI.PhysicsApi;
using HonkAPI.Props;
using HonkAPI.Weather;
using HonkAPI.World;

namespace HonkAPI
{
    // Facade members for the APIs added after the first Honk.cs. Same entry points, same static layout.
    public static partial class Honk
    {
        public static partial class Goose
        {
            public static void HonkSound(global::Goose goose, float pitch = 1f) { GooseAPI.Honk(goose, pitch); }
            public static global::Goose Get(int playerNumber) { return GooseAPI.Get(playerNumber); }
            public static void SetAccelerationMultiplier(float m) { GooseAPI.SetAccelerationMultiplier(m); }
            public static void SetTurnMultiplier(float m) { GooseAPI.SetTurnMultiplier(m); }
            public static bool IsSwimming(global::Goose g) { return GooseAPI.IsSwimming(g); }
            public static bool IsRunning(global::Goose g) { return GooseAPI.IsRunning(g); }
            public static bool IsFlapping(global::Goose g) { return GooseAPI.IsFlapping(g); }
            public static bool IsCarrying(global::Goose g) { return GooseAPI.IsCarrying(g); }
            public static bool IsDragging(global::Goose g) { return GooseAPI.IsDragging(g); }
            public static Vector3 GetVelocity(global::Goose g) { return GooseAPI.GetVelocity(g); }
            public static void Launch(global::Goose g, Vector3 impulse) { GooseAPI.Launch(g, impulse); }
            public static void Freeze(global::Goose g, bool frozen) { GooseAPI.Freeze(g, frozen); }
            public static void SetGravityEnabled(global::Goose g, bool enabled) { GooseAPI.SetGravityEnabled(g, enabled); }
            public static void SetAlwaysSwimming(global::Goose g, bool swim) { GooseAPI.SetAlwaysSwimming(g, swim); }
            public static void SetWingsOut(global::Goose g, bool wingsOut) { GooseAPI.SetWingsOut(g, wingsOut); }
            public static void SetNeckExtended(global::Goose g, bool extended) { GooseAPI.SetNeckExtended(g, extended); }
            public static void SetVisible(global::Goose g, bool visible) { GooseAPI.SetVisible(g, visible); }
            public static void SetMaxMultiplayerDistance(float distance) { GooseAPI.SetMaxMultiplayerDistance(distance); }
        }

        public static partial class Props
        {
            public static List<global::Prop> FindAll(string nameFilter) { return PropAPI.FindAll(nameFilter); }
            public static void SetMass(global::Prop p, float mass) { PropAPI.SetMass(p, mass); }
            public static void Freeze(global::Prop p, bool frozen) { PropAPI.Freeze(p, frozen); }
            public static void SetScale(global::Prop p, float scale) { PropAPI.SetScale(p, scale); }
            public static void Tint(global::Prop p, Color color) { PropAPI.Tint(p, color); }
            public static void SetVisible(global::Prop p, bool visible) { PropAPI.SetVisible(p, visible); }
            public static void Respawn(global::Prop p) { PropAPI.Respawn(p); }
            public static bool IsHeld(global::Prop p) { return PropAPI.IsHeld(p); }
            public static List<global::Prop> GetHeld() { return PropAPI.GetHeldProps(); }
        }

        public static partial class Physics
        {
            public static List<global::Prop> PropsInRadius(Vector3 center, float radius) { return PhysicsAPI.GetPropsInRadius(center, radius); }
            public static List<global::Brain> NPCsInRadius(Vector3 center, float radius) { return PhysicsAPI.GetNPCsInRadius(center, radius); }
            public static void Shockwave(Vector3 origin, float force, float radius, bool affectGeese = true, bool scareNPCs = true) { PhysicsAPI.Shockwave(origin, force, radius, affectGeese, scareNPCs); }
        }

        public static partial class Cheats
        {
            public static void CompleteGoal(string goalName) { CheatsAPI.CompleteGoal(goalName); }
            public static void FreezeNPCs(bool frozen) { CheatsAPI.FreezeNPCs(frozen); }
            public static void ScareAllNPCs(float radius = 0f) { CheatsAPI.ScareAllNPCs(radius); }
        }

        public static partial class Input
        {
            public static bool IsHonkPressed(int playerNumber = 0) { return InputAPI.IsHonkPressed(playerNumber); }
            public static bool GetButton(int player, int actionId) { return InputAPI.GetButton(player, actionId); }
            public static bool GetButtonDown(int player, int actionId) { return InputAPI.GetButtonDown(player, actionId); }
            public static bool GetButtonUp(int player, int actionId) { return InputAPI.GetButtonUp(player, actionId); }
            public static float GetAxis(int player, int actionId) { return InputAPI.GetAxis(player, actionId); }
            public static Vector3? MouseGroundPoint(float planeY = 0f) { return InputAPI.GetMouseGroundPoint(planeY); }
            public static void RegisterChord(KeyCode modifier, KeyCode key, Action callback) { InputAPI.RegisterChord(modifier, key, callback); }
            public static void UnregisterAll() { InputAPI.UnregisterAll(); }
        }

        public static partial class Weather
        {
            public static void SetAmbientIntensity(float i) { WeatherAPI.SetAmbientIntensity(i); }
            public static void SetLinearFog(Color color, float start, float end) { WeatherAPI.SetLinearFog(color, start, end); }
            public static void SetSun(Color color, float intensity) { WeatherAPI.SetSun(color, intensity); }
            public static void SetSunRotation(Vector3 euler) { WeatherAPI.SetSunRotation(euler); }
            public static bool SetSkyTint(Color color) { return WeatherAPI.SetSkyTint(color); }
            public static void ScaleAllLights(float multiplier) { WeatherAPI.ScaleAllLights(multiplier); }
            public static void SetTimeOfDay(float hours) { WeatherAPI.SetTimeOfDay(hours); }
            public static void SetSunset() { WeatherAPI.SetSunset(); }
        }

        public static partial class Camera
        {
            public static void SetDistances(float standard, float zoomedIn, float zoomedOut) { CameraAPI.SetDistances(standard, zoomedIn, zoomedOut); }
            public static void SetOffset(Vector3 offset, Vector3 offsetZoomedIn) { CameraAPI.SetOffset(offset, offsetZoomedIn); }
            public static void SetRotation(Vector3 euler) { CameraAPI.SetRotation(euler); }
            public static void SetDamping(float position, float angle) { CameraAPI.SetDamping(position, angle); }
            public static void SetInstant(bool instant) { CameraAPI.SetInstant(instant); }
            public static void SetUseBounds(bool useBounds) { CameraAPI.SetUseBounds(useBounds); }
            public static void Follow(Transform target) { CameraAPI.Follow(target); }
            public static void SetFrozen(bool frozen) { CameraAPI.SetFrozen(frozen); }
            public static void CatchUp() { CameraAPI.CatchUp(); }
            public static void SetBlur(bool blurred) { CameraAPI.SetBlur(blurred); }
            public static void Shake(float duration, float magnitude) { CameraAPI.Shake(duration, magnitude); }
            public static void Reset() { CameraAPI.Reset(); }
        }

        public static partial class Audio
        {
            public static void PlayPitched(string fmodEvent, Vector3 position, float pitch) { HonkAudioAPI.PlayPitched(fmodEvent, position, pitch); }
            public static bool SetBusVolume(string busPath, float volume) { return HonkAudioAPI.SetBusVolume(busPath, volume); }
            public static float GetBusVolume(string busPath) { return HonkAudioAPI.GetBusVolume(busPath); }
            public static void StopAll() { HonkAudioAPI.StopAllSounds(); }
        }

        public static partial class World
        {
            public static bool IsPaused { get { return WorldAPI.IsPaused; } }
            public static global::GoalListArea CurrentAreaId { get { return WorldAPI.CurrentAreaId; } }
            public static bool TryGetGroundHeight(Vector3 position, out float height, float maxDistance = 200f) { return WorldAPI.TryGetGroundHeight(position, out height, maxDistance); }
            public static bool TryGetNavMeshPoint(Vector3 position, float radius, out Vector3 result) { return WorldAPI.TryGetNavMeshPoint(position, radius, out result); }
            public static Vector3? GetAreaSpawn(global::GoalListArea area) { return WorldAPI.GetAreaSpawn(area); }
        }

        public static partial class Goals
        {
            public static List<global::Goal> GetByArea(global::GoalListArea area) { return GoalRegistry.GetByArea(area); }
            public static List<global::Goal> GetCompleted() { return GoalRegistry.GetCompleted(); }
            public static List<global::Goal> GetRemaining() { return GoalRegistry.GetRemaining(); }
            public static void GetProgress(global::GoalListArea area, out int completed, out int total) { GoalRegistry.GetProgress(area, out completed, out total); }
            public static bool IsAreaComplete(global::GoalListArea area) { return GoalRegistry.IsAreaComplete(area); }
            public static bool MakeReawardable(string goalName) { return GoalRegistry.MakeReawardable(goalName); }
            public static bool Reveal(string goalName) { return GoalRegistry.Reveal(goalName); }
        }

        public static partial class Events
        {
            public static event HonkEvents.AreaChangeHandler OnAreaChange
            {
                add { HonkEvents.OnAreaChange += value; }
                remove { HonkEvents.OnAreaChange -= value; }
            }

            public static event HonkAPI.NPC.BrainStateChangedHandler OnNPCStateChanged
            {
                add { HonkEvents.OnNPCStateChanged += value; }
                remove { HonkEvents.OnNPCStateChanged -= value; }
            }
        }
    }
}
