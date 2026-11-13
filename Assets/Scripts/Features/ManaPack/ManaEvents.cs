using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public struct ManaChangeSuccessEvent : IEventData
    {
        public int newAmount;
        public Entity entity;
    }

    public struct ManaChangeFailedEvent : IEventData
    {
        public Entity entity;
    }
}
