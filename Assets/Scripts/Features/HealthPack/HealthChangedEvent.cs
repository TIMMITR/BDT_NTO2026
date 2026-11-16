using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public struct HealthChangedEvent : IEventData
    {
        public int newAmount;
        public int delta;
        public Entity entity;
    }
}
