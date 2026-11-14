using Features.QuickTalkPack.Data;
using Scellecs.Morpeh;

namespace Features.QuickTalkPack.Events
{
    public struct QuickTalkTriggerEvent : IEventData
    {
        public QuickTalkTrigger trigger;
        public int value;
    }
}
