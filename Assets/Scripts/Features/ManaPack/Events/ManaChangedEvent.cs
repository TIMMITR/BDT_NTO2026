using Scellecs.Morpeh;

namespace Features.ManaPack.Events
{
    public struct ManaChangedEvent : IEventData
    {
        public int newAmount;
        public Entity entity;
    }
}
