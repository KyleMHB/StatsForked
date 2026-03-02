using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public static class Globals
{
    public static class GUI
    {
        internal const float EstimatedInputFieldInnerPadding = 2f;
        internal const float ButtonSubtleContentHoverOffset = 2f;

        public static readonly float Pad = 10f;
        public static readonly float PadSm = 5f;
        public static readonly float PadXs = 3f;
        public static readonly Color TextColorHighlight = new(1f, 0.98f, 0.62f);
        public static readonly Color TextColorSecondary = Color.grey;
        public static float Opacity { get; set; } = 1f;
    }

    public static class Events
    {
        public static event Action? OnResearchCompleted;
        public static event Action<Thing>? ThingSpawned;
        public static event Action<Thing>? ThingDespawned;

        static Events()
        {
            Find.SignalManager.RegisterReceiver(new ResearchCompletedSignalReceiver());
        }

        internal static void NotifyThingSpawned(Thing thing)
        {
            ThingSpawned?.Invoke(thing);
        }

        internal static void NotifyThingDespawned(Thing thing)
        {
            ThingDespawned?.Invoke(thing);
        }

        private sealed class ResearchCompletedSignalReceiver : ISignalReceiver
        {
            public void Notify_SignalReceived(Signal signal)
            {
                if (signal.tag == "ResearchCompleted")
                {
                    OnResearchCompleted?.Invoke();
                }
            }
        }
    }
}
