using Features.StoryTalkPack.Data;
using Features.StoryTalkPack.Events;
using Features.StoryTalkPack.Requests;
using Features.TypingPack.Events;
using Features.TypingPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.StoryTalkPack.Systems
{
    public sealed class StoryTalkPlayerSystem : ISystem
    {
        public World World { get; set; }

        private Request<StoryTalkShowRequest> _showTalkRequest;
        private Request<TypingStartRequest> _typingStartRequest;

        private Event<TypingEndEvent> _typingEndEvent;
        private Event<StoryTalkStartEvent> _talkStartedEvent;
        private Event<StoryTalkFinishEvent> _talkFinishedEvent;

        private readonly StoryTalk _introTalk;

        private StoryTalk _activeTalk;
        private int _lineIndex;

        public StoryTalkPlayerSystem(StoryTalk introTalk)
        {
            _introTalk = introTalk;
        }

        public void OnAwake()
        {
            _showTalkRequest = World.GetRequest<StoryTalkShowRequest>();
            _typingStartRequest = World.GetRequest<TypingStartRequest>();

            _typingEndEvent = World.GetEvent<TypingEndEvent>();
            _talkStartedEvent = World.GetEvent<StoryTalkStartEvent>();
            _talkFinishedEvent = World.GetEvent<StoryTalkFinishEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _showTalkRequest.Consume())
            {
                if (request.talk == null) continue;

                _activeTalk = request.talk;
                _lineIndex = 0;

                _talkStartedEvent.NextFrame(new StoryTalkStartEvent { talk = _activeTalk });

                if (!PlayLine())
                    FinishTalk();
            }

            if (_activeTalk == null) return;

            foreach (var _ in _typingEndEvent.publishedChanges)
            {
                if (_activeTalk == null) break;

                _lineIndex++;

                if (!PlayLine())
                    FinishTalk();
            }
        }

        private bool PlayLine()
        {
            if (_lineIndex >= _activeTalk.Lines.Count)
                return false;

            _typingStartRequest.Publish(new TypingStartRequest { line = _activeTalk.Lines[_lineIndex] });
            return true;
        }

        private void FinishTalk()
        {
            var finished = _activeTalk;
            _activeTalk = null;
            _lineIndex = 0;
            _talkFinishedEvent.NextFrame(new StoryTalkFinishEvent { talk = finished });
        }

        public void Dispose() { }
    }
}
