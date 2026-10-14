using System;
using BepInEx.Logging;
using HonkAPI.Props;
using HonkAPI.Events;

namespace HonkAPI.Core
{
    public class HonkContext
    {
        public string ModId { get; private set; }
        public ManualLogSource Logger { get; private set; }
        public HonkSaveAPI SaveAPI { get; private set; }

        public HonkContext(string modId, ManualLogSource logger)
        {
            ModId = modId;
            Logger = logger;
            SaveAPI = new HonkSaveAPI(modId);
        }
    }
}
