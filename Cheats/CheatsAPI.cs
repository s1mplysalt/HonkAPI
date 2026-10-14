using System;
using System.Collections.Generic;
using UnityEngine;
using HonkAPI.GooseApi;
using HonkAPI.Goals;

namespace HonkAPI.Cheats
{
    public static class CheatsAPI
    {
        public static void SetAllowCheats(bool allow)
        {
            if (GameSettings.currentSettings != null)
            {
                GameSettings.currentSettings.allowCheats = allow;
            }
        }

        public static void CompleteAllGoals()
        {
            List<Goal> all = GoalRegistry.GetAll();
            foreach (Goal g in all)
            {
                if (g != null && !g.awarded)
                {
                    g.AwardGoalDuringGame();
                }
            }
        }

        public static bool TeleportToArea(GoalListArea area)
        {
            return GooseAPI.TeleportToArea(area);
        }

        public static void CompleteGoal(string goalName)
        {
            GoalRegistry.CompleteGoal(goalName);
        }

        /// <summary>Stops or resumes every NPC brain.</summary>
        public static void FreezeNPCs(bool frozen)
        {
            foreach (Brain b in HonkAPI.NPC.NPCAPI.GetAll())
            {
                if (b == null) continue;
                b.enabled = !frozen;
                if (b.agent != null && b.agent.isOnNavMesh) b.agent.isStopped = frozen;
            }
        }

        public static void ScareAllNPCs(float radius = 0f)
        {
            HonkAPI.NPC.NPCAPI.ScareAll(radius);
        }

        public static void SetSuperSpeed(bool enable, float mult = 2.5f)
        {
            GooseAPI.SetSpeedMultiplier(enable ? mult : 1.0f);
        }

        public static void SetGiantGoose(bool enable, float scale = 2.5f)
        {
            GooseAPI.SetScale(enable ? scale : 1.0f);
        }

        public static void SetTinyGoose(bool enable, float scale = 0.35f)
        {
            GooseAPI.SetScale(enable ? scale : 1.0f);
        }
    }
}
