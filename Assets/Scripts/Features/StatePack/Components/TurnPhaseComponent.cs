using Scellecs.Morpeh;

namespace Features.StatePack.Components
{
    public struct TurnPhaseComponent : IComponent
    {
        public TurnPhase phase;
    }

    public enum TurnPhase
    {
        Player,
        Enemy,
        Events,
        None
    }
}
