using Features.StatePack.Components;
using Scellecs.Morpeh;

namespace Features.StatePack.Events
{
    public struct PhaseChangedEvent : IEventData
    {
        public TurnPhase phase;
        public TurnType type;

        public PhaseChangedEvent(TurnPhase phase, TurnType type)
        {
            this.phase = phase;
            this.type = type;
        }
    }

    public enum TurnType
    {
        Start,
        End
    }
}
