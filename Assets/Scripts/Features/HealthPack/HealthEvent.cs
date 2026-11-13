using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public struct HealthEvent : IEventData
    {
        public int newAmount;
        public int delta;
        public Entity entity;
    }
}
