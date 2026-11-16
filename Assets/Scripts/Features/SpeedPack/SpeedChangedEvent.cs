using Scellecs.Morpeh;

namespace Features.SpeedPack
{
    public struct SpeedChangedEvent : IEventData
    {
        public int newAmount;
        public int delta;
        public Entity entity;
    }
}
