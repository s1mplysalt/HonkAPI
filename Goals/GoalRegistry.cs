using System;
using System.Collections.Generic;
using UnityEngine;

namespace HonkAPI.Goals
{
    public static class GoalRegistry
    {
        public static List<Goal> GetAll()
        {
            if (GoalManager.instance != null && GoalManager.instance.goals != null)
            {
                return GoalManager.instance.goals;
            }
            return new List<Goal>();
        }

        public static Goal Get(string goalName)
        {
            if (GoalManager.instance == null || GoalManager.instance.goals == null) return null;
            foreach (Goal g in GoalManager.instance.goals)
            {
                if (g != null && g.goalInfo.goalName == goalName)
                {
                    return g;
                }
            }
            return null;
        }

        public static void CompleteGoal(string goalName)
        {
            Goal g = Get(goalName);
            if (g != null)
            {
                g.AwardGoalDuringGame();
            }
            else
            {
                SwitchEventManager.TriggerEvent("award-" + goalName);
            }
        }

        public static bool IsCompleted(string goalName)
        {
            Goal g = Get(goalName);
            if (g != null)
            {
                return g.awarded;
            }
            return SaveGameData.GetBoolValue(goalName);
        }

        public static void AddCustomGoal(string goalName, string descriptionKey, GoalListArea area)
        {
            if (GoalManager.instance == null) return;
            GoalInfo info = new GoalInfo(goalName, descriptionKey, area, 0);
            GoalManager.instance.AddGoal(info);
        }

        public static void UnlockExtraPages()
        {
            if (GoalManager.instance != null)
            {
                GoalManager.UnlockExtraGoals();
            }
        }

        public static List<Goal> GetByArea(GoalListArea area)
        {
            List<Goal> result = new List<Goal>();
            foreach (Goal g in GetAll())
            {
                if (g != null && g.goalInfo.goalList == area) result.Add(g);
            }
            return result;
        }

        public static List<Goal> GetCompleted()
        {
            List<Goal> result = new List<Goal>();
            foreach (Goal g in GetAll())
            {
                if (g != null && g.awarded) result.Add(g);
            }
            return result;
        }

        public static List<Goal> GetRemaining()
        {
            List<Goal> result = new List<Goal>();
            foreach (Goal g in GetAll())
            {
                if (g != null && !g.awarded) result.Add(g);
            }
            return result;
        }

        /// <summary>Counts finished and total goals in an area.</summary>
        public static void GetProgress(GoalListArea area, out int completed, out int total)
        {
            completed = 0;
            total = 0;
            foreach (Goal g in GetByArea(area))
            {
                total++;
                if (g.awarded) completed++;
            }
        }

        public static bool IsAreaComplete(GoalListArea area)
        {
            int done, total;
            GetProgress(area, out done, out total);
            return total > 0 && done == total;
        }

        /// <summary>Allows a goal that was already awarded to be awarded again.</summary>
        public static bool MakeReawardable(string goalName)
        {
            Goal g = Get(goalName);
            if (g == null) return false;
            g.SetToReawardable();
            return true;
        }

        public static bool Reveal(string goalName)
        {
            Goal g = Get(goalName);
            if (g == null) return false;
            g.Unhide();
            return true;
        }
    }
}
