using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public struct HealthChangeEvent : IEventData
    {
        public Entity entity;
        public int newAmount;
    }
}