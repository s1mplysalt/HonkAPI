using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace HonkAPI.NPC
{
    /// <summary>
    /// Every state in BrainStates.allStates. The numeric value is the index in that array,
    /// which is also the priority: 0 is evaluated first and wins over higher numbers.
    /// </summary>
    public enum HumanState
    {
        TaskUltimatePriority = 0,
        ReactionCloseSurprise = 1,
        ReactionTakenAback = 2,
        ReactionStartled = 3,
        ReactionJustGotScared = 4,
        ReactionLockedOut = 5,
        ReactionTheftShock = 6,
        ReactionDisguiseSurprise = 7,
        ReactionGooseInABox = 8,
        ReactionBecomePoseGrumpy = 9,
        ReactionSwitchShock = 10,
        ReactionTugOfWarOver = 11,
        ReactionWhoops = 12,
        ReactionVaseSad = 13,
        StandUp = 14,
        Door = 15,
        TaskSuperPriority = 16,
        SwitchImportant = 17,
        TaskHighPriority = 18,
        CowardHide = 19,
        Scared = 20,
        TugOfWar = 21,
        BackOff = 22,
        ReactionINeedToHerd = 23,
        Herd = 24,
        ReactionLostWhileHerding = 25,
        ReactionSittingDefend = 26,
        Defend = 27,
        ReactionWetFullBody = 28,
        ReactionWet = 29,
        Shoo = 30,
        ReactionSnatchFromGoose = 31,
        TidyImportant = 32,
        ReactionPostScared = 33,
        ReactionGooseHasMyItem = 34,
        Chase = 35,
        ItemSearchImportant = 36,
        ReactionSuspiciousListen = 37,
        ReactionHeardNoise = 38,
        ReactionThreaten = 39,
        ReactionCantFindGoose = 40,
        ReactionCantFindItem = 41,
        ReactionFoundYou = 42,
        ReactionDustOff = 43,
        ReactionPeering = 44,
        ReactionInspectItem = 45,
        HonkSuspicious = 46,
        Dislodge = 47,
        ReactionSittingHeadCheck = 48,
        Search = 49,
        TaskLowPriority = 50,
        AnnoyingSwitch = 51,
        Switch = 52,
        TaskVeryLowPriority = 53,
        AdoptProp = 54,
        ItemSearch = 55,
        Tidy = 56,
        Guard = 57,
        Watch = 58,
        PropJob = 59,
        Visit = 60,
        Idle = 61
    }

    public delegate void BrainStateChangedHandler(Brain npc, HumanState? from, HumanState? to);

    /// <summary>
    /// Direct control over the NPC state machine (BrainStates). The game picks the highest-priority
    /// state whose isDesired flag is set each frame; this API can read that, nudge it (Request),
    /// override it completely (Force), or remove states from consideration (Suppress).
    /// </summary>
    public static class BrainStateAPI
    {
        public const int StateCount = 62;

        private class ForceLock
        {
            public int stateIndex;
            public float expiresAt; // <= 0 means no expiry
        }

        private static readonly Dictionary<Brain, ForceLock> forced = new Dictionary<Brain, ForceLock>();
        private static readonly Dictionary<Brain, HashSet<int>> suppressed = new Dictionary<Brain, HashSet<int>>();
        private static readonly HashSet<int> suppressedGlobal = new HashSet<int>();
        private static MethodInfo swapToMethod;

        public static event BrainStateChangedHandler OnStateChanged;

        // ---------------------------------------------------------------- queries

        public static IHumanState GetState(Brain npc, HumanState id)
        {
            if (npc == null || npc.brainStates == null || npc.brainStates.allStates == null) return null;
            int i = (int)id;
            if (i < 0 || i >= npc.brainStates.allStates.Length) return null;
            return npc.brainStates.allStates[i];
        }

        public static T GetState<T>(Brain npc, HumanState id) where T : class
        {
            return GetState(npc, id) as T;
        }

        public static ReactionState GetReaction(Brain npc, ReactionName name)
        {
            if (npc == null || npc.allReactionStates == null) return null;
            foreach (ReactionState r in npc.allReactionStates)
            {
                if (r != null && r.reactionName == name) return r;
            }
            return null;
        }

        public static int IndexOf(Brain npc, IHumanState state)
        {
            if (npc == null || npc.brainStates == null || state == null) return -1;
            return Array.IndexOf(npc.brainStates.allStates, state);
        }

        public static HumanState? GetCurrent(Brain npc)
        {
            if (npc == null) return null;
            int i = IndexOf(npc, npc.currentState);
            if (i < 0) return null;
            return (HumanState)i;
        }

        public static bool IsIn(Brain npc, HumanState id)
        {
            if (npc == null) return false;
            return npc.currentState != null && ReferenceEquals(npc.currentState, GetState(npc, id));
        }

        public static bool IsDesired(Brain npc, HumanState id)
        {
            IHumanState s = GetState(npc, id);
            return s != null && s.isDesired;
        }

        public static List<HumanState> GetDesired(Brain npc)
        {
            List<HumanState> list = new List<HumanState>();
            if (npc == null || npc.brainStates == null) return list;
            IHumanState[] all = npc.brainStates.allStates;
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != null && all[i].isDesired) list.Add((HumanState)i);
            }
            return list;
        }

        public static string Describe(Brain npc)
        {
            if (npc == null) return "(null brain)";
            StringBuilder sb = new StringBuilder();
            HumanState? cur = GetCurrent(npc);
            sb.Append(npc.name).Append(" current=").Append(cur.HasValue ? cur.Value.ToString() : "unknown");
            if (IsForced(npc)) sb.Append(" [forced]");
            sb.Append(" desired=");
            foreach (HumanState s in GetDesired(npc)) sb.Append(s).Append(' ');
            return sb.ToString();
        }

        // ---------------------------------------------------------------- requesting

        /// <summary>Marks the state as desired. The game still arbitrates by priority.</summary>
        public static void Request(Brain npc, HumanState id)
        {
            IHumanState s = GetState(npc, id);
            if (s != null) s.isDesired = true;
        }

        public static void CancelRequest(Brain npc, HumanState id)
        {
            IHumanState s = GetState(npc, id);
            if (s != null) s.isDesired = false;
        }

        public static void ClearAllRequests(Brain npc)
        {
            if (npc == null || npc.brainStates == null) return;
            foreach (IHumanState s in npc.brainStates.allStates)
            {
                if (s != null) s.isDesired = false;
            }
        }

        /// <summary>Triggers a reaction through the game's own Decides.RequestReaction.</summary>
        public static ReactionState TriggerReaction(Brain npc, ReactionName name)
        {
            if (npc == null) return null;
            ReactionState r = GetReaction(npc, name);
            if (r != null) r.isDesired = true;
            return r;
        }

        // ---------------------------------------------------------------- forcing

        /// <summary>
        /// Switches the NPC into the state immediately (running Exit/Enter) and holds it there,
        /// ignoring normal priority arbitration, until Release is called or the duration elapses.
        /// A duration of zero or less holds indefinitely.
        /// </summary>
        public static bool Force(Brain npc, HumanState id, float duration = 0f)
        {
            IHumanState state = GetState(npc, id);
            if (state == null) return false;
            ForceLock l = new ForceLock();
            l.stateIndex = (int)id;
            l.expiresAt = duration > 0f ? Time.time + duration : 0f;
            forced[npc] = l;
            SwapTo(npc, state);
            return true;
        }

        public static void Release(Brain npc)
        {
            if (npc == null) return;
            ForceLock l;
            if (forced.TryGetValue(npc, out l))
            {
                forced.Remove(npc);
                IHumanState s = GetState(npc, (HumanState)l.stateIndex);
                if (s != null) s.isDesired = false;
            }
        }

        public static void ReleaseAll()
        {
            List<Brain> keys = new List<Brain>(forced.Keys);
            foreach (Brain b in keys) Release(b);
        }

        public static bool IsForced(Brain npc)
        {
            return npc != null && forced.ContainsKey(npc);
        }

        // ---------------------------------------------------------------- suppression

        public static void Suppress(Brain npc, HumanState id)
        {
            if (npc == null) return;
            HashSet<int> set;
            if (!suppressed.TryGetValue(npc, out set))
            {
                set = new HashSet<int>();
                suppressed[npc] = set;
            }
            set.Add((int)id);
        }

        public static void Unsuppress(Brain npc, HumanState id)
        {
            if (npc == null) return;
            HashSet<int> set;
            if (suppressed.TryGetValue(npc, out set)) set.Remove((int)id);
        }

        /// <summary>Prevents the state from being chosen on every NPC.</summary>
        public static void SuppressEverywhere(HumanState id)
        {
            suppressedGlobal.Add((int)id);
        }

        public static void UnsuppressEverywhere(HumanState id)
        {
            suppressedGlobal.Remove((int)id);
        }

        public static bool IsSuppressed(Brain npc, HumanState id)
        {
            if (suppressedGlobal.Contains((int)id)) return true;
            HashSet<int> set;
            return npc != null && suppressed.TryGetValue(npc, out set) && set.Contains((int)id);
        }

        public static void ClearSuppression()
        {
            suppressed.Clear();
            suppressedGlobal.Clear();
        }

        // ---------------------------------------------------------------- internals used by patches

        internal static void SwapTo(Brain npc, IHumanState state)
        {
            if (npc == null || npc.brainStates == null || state == null) return;
            if (ReferenceEquals(npc.currentState, state)) return;
            if (swapToMethod == null)
            {
                swapToMethod = typeof(BrainStates).GetMethod("SwapTo", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            if (swapToMethod != null) swapToMethod.Invoke(npc.brainStates, new object[] { state });
        }

        /// <summary>Returns true when the normal state arbitration should be skipped for this brain.</summary>
        internal static bool TryHold(Brain npc)
        {
            ForceLock l;
            if (!forced.TryGetValue(npc, out l)) return false;
            if (l.expiresAt > 0f && Time.time >= l.expiresAt)
            {
                Release(npc);
                return false;
            }
            IHumanState s = GetState(npc, (HumanState)l.stateIndex);
            if (s == null)
            {
                forced.Remove(npc);
                return false;
            }
            if (!ReferenceEquals(npc.currentState, s)) SwapTo(npc, s);
            return true;
        }

        internal static void ApplySuppression(Brain npc)
        {
            if (suppressedGlobal.Count == 0 && suppressed.Count == 0) return;
            IHumanState[] all = npc.brainStates.allStates;
            foreach (int i in suppressedGlobal)
            {
                if (i >= 0 && i < all.Length && all[i] != null && !ReferenceEquals(all[i], npc.currentState)) all[i].isDesired = false;
            }
            HashSet<int> set;
            if (suppressed.TryGetValue(npc, out set))
            {
                foreach (int i in set)
                {
                    if (i >= 0 && i < all.Length && all[i] != null && !ReferenceEquals(all[i], npc.currentState)) all[i].isDesired = false;
                }
            }
        }

        internal static void RaiseStateChanged(Brain npc, IHumanState from, IHumanState to)
        {
            if (OnStateChanged == null) return;
            int f = IndexOf(npc, from);
            int t = IndexOf(npc, to);
            HumanState? hf = f >= 0 ? (HumanState?)(HumanState)f : null;
            HumanState? ht = t >= 0 ? (HumanState?)(HumanState)t : null;
            try { OnStateChanged(npc, hf, ht); }
            catch (Exception e) { Debug.LogError("[HonkAPI] OnStateChanged handler threw: " + e); }
        }
    }
}
