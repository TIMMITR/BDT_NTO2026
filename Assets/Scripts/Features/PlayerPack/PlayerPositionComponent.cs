using Scellecs.Morpeh;

namespace Features.PlayerPack
{
    public struct PlayerPositionComponent : IComponent
    {
        public PlayerPositionState positionState;
    }

    public enum PlayerPositionState
    {
        Idle,
        Battle
    }
}