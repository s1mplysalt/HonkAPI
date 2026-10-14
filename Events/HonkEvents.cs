using System;
using UnityEngine;
using HonkAPI.NPC;

namespace HonkAPI.Events
{
    public static class HonkEvents
    {
        public delegate void GooseSpawnHandler(Goose goose);
        public delegate void HonkHandler(Goose goose, bool isGlobal);
        public delegate void PropGrabHandler(Goose goose, Prop prop);
        public delegate void PropDropHandler(Goose goose, Prop prop);
        public delegate void NPCScaredHandler(Brain npc);
        public delegate void GoalAwardHandler(Goal goal);
        public delegate void AreaChangeHandler(GoalListArea newArea);
        public delegate void TickHandler();

        public static event GooseSpawnHandler OnGooseSpawn;
        public static event HonkHandler OnHonk;
        public static event PropGrabHandler OnGrabProp;
        public static event PropDropHandler OnDropProp;
        public static event NPCScaredHandler OnNPCScared;
        public static event GoalAwardHandler OnGoalAward;
        public static event AreaChangeHandler OnAreaChange;
        public static event TickHandler OnUpdate;
        public static event TickHandler OnGUI;

        /// <summary>Raised whenever any NPC's brain switches state.</summary>
        public static event BrainStateChangedHandler OnNPCStateChanged
        {
            add { BrainStateAPI.OnStateChanged += value; }
            remove { BrainStateAPI.OnStateChanged -= value; }
        }

        public static void InvokeGooseSpawn(Goose goose)
        {
            if (OnGooseSpawn != null)
            {
                OnGooseSpawn(goose);
            }
        }

        public static void InvokeHonk(Goose goose, bool isGlobal)
        {
            if (OnHonk != null)
            {
                OnHonk(goose, isGlobal);
            }
        }

        public static void InvokeGrabProp(Goose goose, Prop prop)
        {
            if (OnGrabProp != null)
            {
                OnGrabProp(goose, prop);
            }
        }

        public static void InvokeDropProp(Goose goose, Prop prop)
        {
            if (OnDropProp != null)
            {
                OnDropProp(goose, prop);
            }
        }

        public static void InvokeNPCScared(Brain npc)
        {
            if (OnNPCScared != null)
            {
                OnNPCScared(npc);
            }
        }

        public static void InvokeGoalAward(Goal goal)
        {
            if (OnGoalAward != null)
            {
                OnGoalAward(goal);
            }
        }

        public static void InvokeAreaChange(GoalListArea area)
        {
            if (OnAreaChange != null)
            {
                OnAreaChange(area);
            }
        }

        public static void InvokeUpdate()
        {
            if (OnUpdate != null)
            {
                OnUpdate();
            }
        }

        public static void InvokeGUI()
        {
            if (OnGUI != null)
            {
                OnGUI();
            }
        }
    }
}
