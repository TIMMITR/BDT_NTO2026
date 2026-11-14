using Features.StoryTalkPack.Data;
using Scellecs.Morpeh;

namespace Features.StoryTalkPack.Events
{
    public struct StoryTalkFinishEvent : IEventData
    {
        public StoryTalk talk;
    }
}
