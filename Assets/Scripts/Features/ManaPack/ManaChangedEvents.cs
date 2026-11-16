using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public struct ManaChangedSuccessEvent : IEventData
    {
        public int newAmount;
        public Entity entity;
    }

    public struct ManaChangedFailedEvent : IEventData
    {
        public Entity entity;
    }
}
