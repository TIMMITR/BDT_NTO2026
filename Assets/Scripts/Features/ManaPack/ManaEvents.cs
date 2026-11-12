using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public struct ManaChangeSuccessEvent : IEventData
    {
        public int newAmount;
    }
    
    public struct ManaChangeFailedEvent : IEventData { }
}