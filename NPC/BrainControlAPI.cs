using System;
using System.Collections.Generic;
using UnityEngine;

namespace HonkAPI.NPC
{
    /// <summary>Low-level access to the rest of an NPC's Brain: targets, speeds, permissions, awareness and reaction tuning.</summary>
    public static class BrainControlAPI
    {
        // ---------------------------------------------------------------- identity

        public static Folk GetFolk(Brain npc)
        {
            return npc.specificPerson.who;
        }

        public static Brain FindByFolk(Folk who)
        {
            foreach (Brain b in NPCAPI.GetAll())
            {
                if (b != null && b.specificPerson != null && b.specificPerson.who == who) return b;
            }
            return null;
        }

        public static void SetVerboseLogging(Brain npc, bool enabled)
        {
            if (npc != null && npc.specificPerson != null) npc.specificPerson.useVerboseLogging = enabled;
        }

        // ---------------------------------------------------------------- targeting and movement

        public static void SetTarget(Brain npc, GameObject target)
        {
            if (npc != null) npc.SetTarget(target);
        }

        public static void SetTarget(Brain npc, Vector3 position, bool reaching)
        {
            if (npc != null) npc.SetTarget(position, reaching);
        }

        public static GameObject GetTarget(Brain npc)
        {
            return npc != null ? npc.target : null;
        }

        public static void SetLookTarget(Brain npc, GameObject target)
        {
            if (npc != null) npc.lookTarget = target;
        }

        public static void FaceTowards(Brain npc, Vector3 position)
        {
            if (npc != null && npc.does != null) npc.does.RotateTowards(position);
        }

        public static void SetCasualSpeed(Brain npc, float speed)
        {
            if (npc != null) npc.casualSpeed = speed;
        }

        public static void SetHurriedSpeed(Brain npc, float speed)
        {
            if (npc != null) npc.hurriedSpeed = speed;
        }

        public static void SetUrgent(Brain npc, bool urgent)
        {
            if (npc != null && npc.does != null) npc.does.SetUrgent(urgent);
        }

        public static bool IsUrgent(Brain npc)
        {
            return npc != null && npc.does != null && npc.does.isUrgent;
        }

        public static void Stop(Brain npc)
        {
            if (npc == null) return;
            npc.SetTarget(npc.transform.position);
            if (npc.agent != null && npc.agent.isOnNavMesh) npc.agent.ResetPath();
        }

        // ---------------------------------------------------------------- behavior permissions

        public static void SetAllowTruckJobs(Brain npc, bool v) { if (npc != null) npc.allowTruckJobs = v; }
        public static void SetAllowVisiting(Brain npc, bool v) { if (npc != null) npc.allowVisiting = v; }
        public static void SetAllowPropJobs(Brain npc, bool v) { if (npc != null) npc.allowPropJobs = v; }
        public static void SetAvoidVisiting(Brain npc, bool v) { if (npc != null) npc.avoidVisiting = v; }
        public static void SetIgnoreAsCameraTarget(Brain npc, bool v) { if (npc != null) npc.ignoreAsCameraTarget = v; }
        public static void SetPickUpRange(Brain npc, float r) { if (npc != null) npc.pickUpRange = r; }

        // ---------------------------------------------------------------- awareness

        public static bool CanSeeGoose(Brain npc)
        {
            return npc != null && npc.knows != null && npc.knows.closestKnowsGoose != null && npc.knows.closestKnowsGoose.isSeen;
        }

        public static bool LostTrackOfGoose(Brain npc)
        {
            return npc != null && npc.knows != null && npc.knows.closestKnowsGoose != null && npc.knows.closestKnowsGoose.locationUnknown;
        }

        public static Vector3 GetAssumedGoosePosition(Brain npc)
        {
            if (npc == null || npc.knows == null || npc.knows.closestKnowsGoose == null) return Vector3.zero;
            return npc.knows.closestKnowsGoose.assumedPosition;
        }

        public static float GetAssumedGooseDistance(Brain npc)
        {
            if (npc == null || npc.knows == null || npc.knows.closestKnowsGoose == null) return float.MaxValue;
            return npc.knows.closestKnowsGoose.assumedDistance;
        }

        /// <summary>Overwrites what the NPC believes about where the goose is.</summary>
        public static void SetAssumedGoosePosition(Brain npc, Vector3 position)
        {
            if (npc == null || npc.knows == null || npc.knows.closestKnowsGoose == null) return;
            npc.knows.closestKnowsGoose.assumedPosition = position;
            npc.knows.closestKnowsGoose.locationUnknown = false;
        }

        public static void ForgetGoose(Brain npc)
        {
            if (npc == null || npc.knows == null || npc.knows.closestKnowsGoose == null) return;
            npc.knows.closestKnowsGoose.BecomeLocationUnknown();
        }

        public static bool IsWet(Brain npc)
        {
            return npc != null && npc.wettable != null && npc.wet;
        }

        // ---------------------------------------------------------------- reaction tuning

        public class ReactionOptions
        {
            public bool? ShouldDrop;
            public bool? LookAtGooseFirst;
            public bool? StareAtGoose;
            public bool? GiveUpIfCantSeeGoose;
            public Transform StareAt;
            public Ideal ImagineThis;
        }

        /// <summary>Adjusts a reaction's behavior and optionally triggers it.</summary>
        public static ReactionState ConfigureReaction(Brain npc, ReactionName name, ReactionOptions options, bool trigger)
        {
            ReactionState r = BrainStateAPI.GetReaction(npc, name);
            if (r == null) return null;
            if (options != null)
            {
                if (options.ShouldDrop.HasValue) r.shouldDrop = options.ShouldDrop.Value;
                if (options.LookAtGooseFirst.HasValue) r.shouldLookAtGooseFirst = options.LookAtGooseFirst.Value;
                if (options.StareAtGoose.HasValue) r.shouldStareAtGoose = options.StareAtGoose.Value;
                if (options.GiveUpIfCantSeeGoose.HasValue) r.giveUpIfCantSeeGoose = options.GiveUpIfCantSeeGoose.Value;
                if (options.StareAt != null) r.shouldStareAtThis = options.StareAt;
                if (options.ImagineThis != null) r.imagineThis = options.ImagineThis;
            }
            if (trigger) r.isDesired = true;
            return r;
        }
    }
}
