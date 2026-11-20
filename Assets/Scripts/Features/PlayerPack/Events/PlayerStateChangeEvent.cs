using Scellecs.Morpeh;

namespace Features.PlayerPack.Events
{
    public struct PlayerStateChangeEvent : IEventData
    {
        public PlayerPositionState newPositionState;
    }
}