using HarmonyLib;

namespace HonkAPI.NPC
{
    [HarmonyPatch(typeof(BrainStates), "SwapTo")]
    internal static class BrainStatesSwapToPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Brain ___brain, out IHumanState __state)
        {
            __state = ___brain != null ? ___brain.currentState : null;
        }

        [HarmonyPostfix]
        public static void Postfix(Brain ___brain, IHumanState newState, IHumanState __state)
        {
            if (___brain == null) return;
            BrainStateAPI.RaiseStateChanged(___brain, __state, newState);
        }
    }

    [HarmonyPatch(typeof(BrainStates), "Update")]
    internal static class BrainStatesUpdatePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Brain ___brain)
        {
            if (___brain == null) return true;

            if (BrainStateAPI.TryHold(___brain))
            {
                // Skip priority arbitration; just keep the forced state running.
                ___brain.currentState.Update();
                return false;
            }

            BrainStateAPI.ApplySuppression(___brain);
            return true;
        }
    }
}
