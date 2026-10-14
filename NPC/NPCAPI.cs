using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HonkAPI.NPC
{
    public enum PunctuationType
    {
        Scared = 100,
        Alert = 200,
        QuestionMark = 300,
        FoundYou = 400,
        Grumpy = 500
    }

    public static class NPCAPI
    {
        public static List<Brain> GetAll()
        {
            List<Brain> list = new List<Brain>();
            Brain[] found = UnityEngine.Object.FindObjectsOfType<Brain>();
            if (found != null)
            {
                list.AddRange(found);
            }
            return list;
        }

        public static Brain GetClosest(Vector3 position)
        {
            List<Brain> all = GetAll();
            Brain closest = null;
            float minDist = float.MaxValue;
            foreach (Brain brain in all)
            {
                if (brain == null) continue;
                float d = Vector3.Distance(brain.transform.position, position);
                if (d < minDist)
                {
                    minDist = d;
                    closest = brain;
                }
            }
            return closest;
        }

        /// <summary>Puts the NPC into the game's ScaredState and keeps it there for the given time (0 = until Calm).</summary>
        public static void Scare(Brain npc, float duration = 8f)
        {
            if (npc == null) return;
            try
            {
                BrainStateAPI.Force(npc, HumanState.Scared, duration);
                if (npc.punctuates != null)
                {
                    npc.punctuates.SetPunctuation(PunctuationMap.scared);
                }
                if (npc.does != null)
                {
                    npc.does.SetUrgent(true);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[HonkAPI] Scare failed: " + e.Message);
            }
        }

        public static void ScareAll(float radius = 50f)
        {
            Vector3 center = Vector3.zero;
            if (HonkAPI.GooseApi.GooseAPI.Current != null)
            {
                center = HonkAPI.GooseApi.GooseAPI.Current.transform.position;
            }
            foreach (Brain brain in GetAll())
            {
                if (brain == null) continue;
                if (radius <= 0f || Vector3.Distance(brain.transform.position, center) <= radius)
                {
                    Scare(brain);
                }
            }
        }

        public static void SetPunctuation(Brain npc, PunctuationType type)
        {
            if (npc == null || npc.punctuates == null) return;
            try
            {
                npc.punctuates.SetPunctuation((int)type);
            }
            catch (Exception)
            {
            }
        }

        public static void Alert(Brain npc)
        {
            SetPunctuation(npc, PunctuationType.Alert);
        }

        public static void Confuse(Brain npc)
        {
            SetPunctuation(npc, PunctuationType.QuestionMark);
        }

        public static void MakeGrumpy(Brain npc)
        {
            SetPunctuation(npc, PunctuationType.Grumpy);
        }

        public static void ClearPunctuation(Brain npc)
        {
            if (npc == null || npc.punctuates == null) return;
            try
            {
                npc.punctuates.ClearPunctuation();
            }
            catch (Exception)
            {
            }
        }

        public static void WalkTo(Brain npc, Vector3 destination)
        {
            if (npc == null) return;
            try
            {
                if (npc.agent != null && npc.agent.isOnNavMesh)
                {
                    npc.agent.SetDestination(destination);
                }
                npc.SetTarget(destination);
            }
            catch (Exception)
            {
            }
        }

        public static void MakeDropHeld(Brain npc)
        {
            if (npc == null || npc.holder == null) return;
            try
            {
                Prop held = npc.holder.holding;
                if (held != null)
                {
                    npc.holder.Drop();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void SetSpeed(Brain npc, float multiplier)
        {
            if (npc == null) return;
            float factor = Mathf.Max(0.1f, multiplier);
            npc.casualSpeed = 0.675f * factor;
            npc.hurriedSpeed = 1.8f * factor;
            if (npc.agent != null)
            {
                npc.agent.speed = npc.hurriedSpeed;
            }
        }

        public static void SetWalkSpeed(Brain npc, float speed)
        {
            SetSpeed(npc, speed);
        }

        public static void Distract(Brain npc, Vector3 lookAtPosition)
        {
            if (npc == null) return;
            try
            {
                WalkTo(npc, lookAtPosition);
                Confuse(npc);
            }
            catch (Exception)
            {
            }
        }

        public static void Teleport(Brain npc, Vector3 position)
        {
            if (npc == null) return;
            try
            {
                if (npc.agent != null && npc.agent.isOnNavMesh)
                {
                    npc.agent.Warp(position);
                }
                else
                {
                    npc.transform.position = position;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>Releases any forced state and returns the NPC to normal decision making.</summary>
        public static void Calm(Brain npc)
        {
            if (npc == null) return;
            try
            {
                BrainStateAPI.Release(npc);
                BrainStateAPI.CancelRequest(npc, HumanState.Scared);
                ClearPunctuation(npc);
                if (npc.does != null)
                {
                    npc.does.SetUrgent(false);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[HonkAPI] Calm failed: " + e.Message);
            }
        }

        public static void CalmAll()
        {
            foreach (Brain brain in GetAll())
            {
                if (brain == null) continue;
                Calm(brain);
            }
        }

        public static void ResetSpeed(Brain npc)
        {
            SetSpeed(npc, 1.0f);
        }

        public static void ResetAllSpeeds()
        {
            foreach (Brain brain in GetAll())
            {
                if (brain == null) continue;
                ResetSpeed(brain);
            }
        }
    }
}
