using System;
using System.Collections.Generic;
using UnityEngine;
using HonkAPI.Audio;
using HonkAPI.Events;

namespace HonkAPI.GooseApi
{
    public static class GooseAPI
    {
        public static Goose Current
        {
            get
            {
                if (GameManager.instance != null && GameManager.instance.allGeese != null && GameManager.instance.allGeese.Count > 0)
                {
                    return GameManager.instance.allGeese[0];
                }
                return UnityEngine.Object.FindObjectOfType<Goose>();
            }
        }

        public static List<Goose> AllGeese
        {
            get
            {
                if (GameManager.instance != null && GameManager.instance.allGeese != null)
                {
                    return GameManager.instance.allGeese;
                }
                List<Goose> list = new List<Goose>();
                Goose[] found = UnityEngine.Object.FindObjectsOfType<Goose>();
                if (found != null)
                {
                    list.AddRange(found);
                }
                return list;
            }
        }

        public static void Honk(float pitch = 1f)
        {
            Honk(Current, pitch);
        }

        /// <summary>
        /// A complete honk: sound (optionally pitched), animation, particles, and NPCs within hearing range react.
        /// </summary>
        public static void Honk(Goose goose, float pitch)
        {
            if (goose == null || goose.gooseHonker == null) return;
            try
            {
                if (Mathf.Approximately(pitch, 1f))
                {
                    goose.gooseHonker.PlayHonkSound(false);
                }
                else
                {
                    HonkAudioAPI.PlayPitched(goose.gooseHonker.honkAudioEvent, goose.head != null ? goose.head.position : goose.transform.position, pitch);
                    HonkEvents.InvokeHonk(goose, false);
                }
                if (goose.gooseEngine != null) goose.gooseEngine.SetTrigger("Honk");
                if (goose.gooseHonker.honkParticleSystem != null) goose.gooseHonker.honkParticleSystem.Play();
                goose.gooseHonker.timeAtLastHonk = Time.time;
                if (goose.honkHearAtDistance != null && GameCollections.Instance != null)
                {
                    foreach (Brain b in GameCollections.Instance.allBrains)
                    {
                        if (b != null && b.knows != null && b.knows.closestKnowsGoose != null && goose.honkHearAtDistance.CanBeHeardBy(b))
                        {
                            b.knows.closestKnowsGoose.HearHonk(goose.honkHearAtDistance);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[HonkAPI] Honk failed: " + e.Message);
            }
        }

        public static void FlapWings()
        {
            Goose goose = Current;
            if (goose == null || goose.gooseWings == null) return;
            try
            {
                goose.gooseWings.buttonIsToggledOn = true;
                goose.gooseWings.isFlapping = true;
            }
            catch (Exception)
            {
            }
        }

        public static void DuckHead(float amount = 1f)
        {
            Goose goose = Current;
            if (goose == null || goose.gooseNeck == null) return;
            try
            {
                goose.gooseNeck.headBobAmount = Mathf.Clamp01(amount);
            }
            catch (Exception)
            {
            }
        }

        public static void Teleport(Vector3 position)
        {
            Goose goose = Current;
            if (goose == null) return;
            try
            {
                if (goose.dragger != null && goose.dragger.active && goose.dragger.holdable != null)
                {
                    Vector3 offset = goose.dragger.holdable.transform.position - goose.transform.position;
                    goose.dragger.holdable.transform.position = position + offset;
                    if (goose.dragger.holdable.rb != null)
                    {
                        goose.dragger.holdable.rb.position = position + offset;
                        goose.dragger.holdable.rb.velocity = Vector3.zero;
                        goose.dragger.holdable.rb.angularVelocity = Vector3.zero;
                    }
                }
                if (goose.goosePoser != null && goose.goosePoser.poserActive)
                {
                    goose.goosePoser.Deactivate();
                }
                goose.transform.position = position;
                if (goose.rb != null)
                {
                    goose.rb.isKinematic = true;
                    goose.rb.velocity = Vector3.zero;
                    goose.rb.angularVelocity = Vector3.zero;
                    goose.rb.position = position;
                    goose.rb.isKinematic = false;
                }
                if (goose.isActiveAndEnabled && goose.gooseCameraHelper != null)
                {
                    goose.gooseCameraHelper.LateUpdate();
                }
                if (GameManager.instance != null && GameManager.instance.cameraManager != null)
                {
                    GameManager.instance.cameraManager.CatchUp();
                }
            }
            catch (Exception)
            {
                goose.transform.position = position;
            }
        }

        public static void Teleport(float x, float y, float z)
        {
            Teleport(new Vector3(x, y, z));
        }

        private static int currentTeleportPointIndex = 0;

        public static bool CycleTeleportPoint(int delta = 1)
        {
            Goose goose = Current;
            if (goose == null) return false;
            if (GameCollections.Instance == null || GameCollections.Instance.allTeleportPoints == null || GameCollections.Instance.allTeleportPoints.Count == 0) return false;

            int count = GameCollections.Instance.allTeleportPoints.Count;
            currentTeleportPointIndex = (currentTeleportPointIndex + delta + count) % count;
            TeleportPoint tp = GameCollections.Instance.allTeleportPoints[currentTeleportPointIndex];
            if (tp != null)
            {
                Teleport(tp.transform.position + Vector3.up * 0.15f);
                return true;
            }
            return false;
        }

        public static bool TeleportToArea(GoalListArea area)
        {
            Goose goose = Current;
            if (goose == null) return false;

            Vector3 targetPos = Vector3.zero;
            bool found = false;

            if (GameCollections.Instance != null && GameCollections.Instance.gooseSpawns != null && GameCollections.Instance.gooseSpawns.ContainsKey(area))
            {
                GooseSpawn sp = GameCollections.Instance.gooseSpawns[area];
                if (sp != null)
                {
                    if (sp.pose != null)
                    {
                        targetPos = sp.pose.transform.position;
                    }
                    else
                    {
                        targetPos = sp.transform.position;
                    }
                    found = true;
                }
            }

            if (!found && GameCollections.Instance != null && GameCollections.Instance.folkRegistry != null)
            {
                Folk targetFolk = Folk.none;
                if (area == GoalListArea.Garden) targetFolk = Folk.gardener;
                else if (area == GoalListArea.Shops) targetFolk = Folk.shopkeeper;
                else if (area == GoalListArea.Backyards) targetFolk = Folk.neighborClean;
                else if (area == GoalListArea.Pub) targetFolk = Folk.pubWoman;

                if (targetFolk != Folk.none && GameCollections.Instance.folkRegistry.ContainsKey(targetFolk))
                {
                    Brain b = GameCollections.Instance.folkRegistry[targetFolk];
                    if (b != null)
                    {
                        targetPos = b.transform.position + b.transform.forward * 1.5f;
                        found = true;
                    }
                }
            }

            if (found)
            {
                targetPos.y += 0.2f;
                Teleport(targetPos);
                return true;
            }

            return false;
        }

        public static void SetScale(float scale)
        {
            Goose goose = Current;
            if (goose == null) return;
            float clamped = Mathf.Max(0.1f, scale);
            goose.bigGoseScalar = clamped;
            goose.transform.localScale = Vector3.one * clamped;
        }

        public static void ResetScale()
        {
            SetScale(1.0f);
        }

        private static GooseTuningData capturedTuningOwner;
        private static GooseTuningSet originalTuning;

        private static GooseTuningData CaptureTuning()
        {
            Goose goose = Current;
            if (goose == null || goose.tuningData == null) return null;
            if (capturedTuningOwner != goose.tuningData)
            {
                capturedTuningOwner = goose.tuningData;
                originalTuning = goose.tuningData.tuningSet;
            }
            return goose.tuningData;
        }

        /// <summary>Scales maximum speed of walking, running and sneaking relative to the game's original values.</summary>
        public static void SetSpeedMultiplier(float multiplier)
        {
            GooseTuningData data = CaptureTuning();
            if (data == null) return;
            float f = Mathf.Max(0.1f, multiplier);
            data.tuningSet.walking.maxSpeed = originalTuning.walking.maxSpeed * f;
            data.tuningSet.running.maxSpeed = originalTuning.running.maxSpeed * f;
            data.tuningSet.sneaking.maxSpeed = originalTuning.sneaking.maxSpeed * f;
        }

        /// <summary>Scales how quickly the goose accelerates and brakes.</summary>
        public static void SetAccelerationMultiplier(float multiplier)
        {
            GooseTuningData data = CaptureTuning();
            if (data == null) return;
            float f = Mathf.Max(0.05f, multiplier);
            data.tuningSet.walking.accelerationUp = originalTuning.walking.accelerationUp * f;
            data.tuningSet.walking.accelerationDown = originalTuning.walking.accelerationDown * f;
            data.tuningSet.running.accelerationUp = originalTuning.running.accelerationUp * f;
            data.tuningSet.running.accelerationDown = originalTuning.running.accelerationDown * f;
            data.tuningSet.sneaking.accelerationUp = originalTuning.sneaking.accelerationUp * f;
            data.tuningSet.sneaking.accelerationDown = originalTuning.sneaking.accelerationDown * f;
        }

        /// <summary>Scales how fast the goose can turn.</summary>
        public static void SetTurnMultiplier(float multiplier)
        {
            GooseTuningData data = CaptureTuning();
            if (data == null) return;
            float f = Mathf.Max(0.05f, multiplier);
            data.tuningSet.walking.turnSpeedAtStationary = originalTuning.walking.turnSpeedAtStationary * f;
            data.tuningSet.walking.turnSpeedAtMaxSpeed = originalTuning.walking.turnSpeedAtMaxSpeed * f;
            data.tuningSet.running.turnSpeedAtStationary = originalTuning.running.turnSpeedAtStationary * f;
            data.tuningSet.running.turnSpeedAtMaxSpeed = originalTuning.running.turnSpeedAtMaxSpeed * f;
            data.tuningSet.sneaking.turnSpeedAtStationary = originalTuning.sneaking.turnSpeedAtStationary * f;
            data.tuningSet.sneaking.turnSpeedAtMaxSpeed = originalTuning.sneaking.turnSpeedAtMaxSpeed * f;
        }

        /// <summary>Restores speed, acceleration and turning to the values the game shipped with.</summary>
        public static void ResetSpeed()
        {
            if (capturedTuningOwner != null)
            {
                capturedTuningOwner.tuningSet = originalTuning;
            }
        }

        // ------------------------------------------------------------ player access and state

        public static Goose Get(int playerNumber)
        {
            foreach (Goose g in AllGeese)
            {
                if (g != null && g.playerNumber == playerNumber) return g;
            }
            return null;
        }

        public static bool IsSwimming(Goose goose) { return goose != null && goose.isSwimming; }
        public static bool IsRunning(Goose goose) { return goose != null && goose.isRunning; }
        public static bool IsFlapping(Goose goose) { return goose != null && goose.gooseWings != null && goose.gooseWings.isFlapping; }
        public static bool IsCarrying(Goose goose) { return goose != null && goose.holder != null && goose.holder.holding != null; }
        public static bool IsDragging(Goose goose) { return goose != null && goose.dragger != null && goose.dragger.active; }

        public static Vector3 GetVelocity(Goose goose)
        {
            return (goose != null && goose.rb != null) ? goose.rb.velocity : Vector3.zero;
        }

        public static void Launch(Goose goose, Vector3 impulse)
        {
            if (goose == null || goose.rb == null) return;
            goose.rb.AddForce(impulse, ForceMode.Impulse);
        }

        /// <summary>Holds the goose in place physically. Input still turns the model.</summary>
        public static void Freeze(Goose goose, bool frozen)
        {
            if (goose == null || goose.rb == null) return;
            if (frozen)
            {
                goose.rb.velocity = Vector3.zero;
                goose.rb.angularVelocity = Vector3.zero;
            }
            goose.rb.isKinematic = frozen;
        }

        public static void SetGravityEnabled(Goose goose, bool enabled)
        {
            if (goose != null && goose.rb != null) goose.rb.useGravity = enabled;
        }

        public static void SetAlwaysSwimming(Goose goose, bool swim)
        {
            if (goose != null) goose.gooseSwimmer.alwaysSwimming = swim;
        }

        /// <summary>Opens or closes the wings as if the flap button were toggled.</summary>
        public static void SetWingsOut(Goose goose, bool out_)
        {
            if (goose == null || goose.gooseWings == null) return;
            goose.gooseWings.buttonIsToggledOn = out_;
            if (!out_) goose.gooseWings.isFlapping = false;
        }

        public static void SetNeckExtended(Goose goose, bool extended)
        {
            if (goose != null && goose.gooseNeck != null) goose.gooseNeck.neckIsToggledOn = extended;
        }

        public static void SetVisible(Goose goose, bool visible)
        {
            if (goose == null) return;
            foreach (Renderer r in goose.GetComponentsInChildren<Renderer>(true))
            {
                if (r != null) r.enabled = visible;
            }
        }

        /// <summary>Distance the goose can wander from its partner in co-op.</summary>
        public static void SetMaxMultiplayerDistance(float distance)
        {
            GooseTuningData data = CaptureTuning();
            if (data != null) data.maxMultiplayerDistance = distance;
        }

        public static Prop GetHeldProp()
        {
            Goose goose = Current;
            if (goose == null || goose.holder == null) return null;
            return goose.holder.holding;
        }

        public static void DropHeldProp()
        {
            Goose goose = Current;
            if (goose == null || goose.holder == null) return;
            try
            {
                goose.holder.Drop();
            }
            catch (Exception)
            {
            }
        }

        public static void GrabProp(Prop prop)
        {
            if (prop == null) return;
            Goose goose = Current;
            if (goose == null || goose.holder == null) return;
            try
            {
                DropHeldProp();
                if (prop.beakHandles != null && prop.beakHandles.Length > 0)
                {
                    goose.holder.PickUp(prop.beakHandles[0]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
