using Features.QuickTalkPack.Data;
using Features.TypingPack.Data;
using Scellecs.Morpeh;

namespace Features.QuickTalkPack.Events
{
    public struct QuickTalkShownEvent : IEventData
    {
        public QuickTalkSet set;
        public DialogueLine line;
    }
}
