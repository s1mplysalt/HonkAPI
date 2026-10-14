using System;

namespace HonkAPI.Core
{
    public interface IHonkMod
    {
        string ModId { get; }
        string ModName { get; }
        string Version { get; }
        void OnInitialize(HonkContext context);
    }
}
