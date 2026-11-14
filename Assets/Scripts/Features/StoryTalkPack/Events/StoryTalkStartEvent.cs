using Features.StoryTalkPack.Data;
using Scellecs.Morpeh;

namespace Features.StoryTalkPack.Events
{
    public struct StoryTalkStartEvent : IEventData
    {
        public StoryTalk talk;
    }
}
