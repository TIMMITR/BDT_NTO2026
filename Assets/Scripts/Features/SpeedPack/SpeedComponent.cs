using Scellecs.Morpeh;

namespace Features.SpeedPack
{
    public struct SpeedComponent : IComponent
    {
        public int currentSpeed;
        public int maxSpeed;

        public SpeedComponent(int value)
        {
            maxSpeed = value;
            currentSpeed = maxSpeed;
        }
    }
}