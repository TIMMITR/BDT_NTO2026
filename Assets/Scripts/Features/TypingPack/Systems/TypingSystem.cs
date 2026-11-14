using Configs;
using Features.TypingPack.Components;
using Features.TypingPack.Events;
using Features.TypingPack.Requests;
using Scellecs.Morpeh;

namespace Features.TypingPack.Systems
{
    public sealed class TypingSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<TypingComponent> _typingStash;

        private Request<TypingStartRequest> _showLineRequest;
        private Request<TypingSkipRequest> _skipRequest;
        private Event<TypingEndEvent> _lineFinishedEvent;

        private readonly WorldSettingsSo _settings;

        public TypingSystem(WorldSettingsSo settings)
        {
            _settings = settings;
        }

        public void OnAwake()
        {
            _filter = World.Filter.With<TypingComponent>().Build();
            _typingStash = World.GetStash<TypingComponent>();

            _showLineRequest = World.GetRequest<TypingStartRequest>();
            _skipRequest = World.GetRequest<TypingSkipRequest>();
            _lineFinishedEvent = World.GetEvent<TypingEndEvent>();

            if (_filter.IsEmpty())
                _typingStash.Add(World.CreateEntity());
        }

        public void OnUpdate(float deltaTime)
        {
            if (_filter.IsEmpty()) return;

            var entity = _filter.First();
            ref var typing = ref _typingStash.Get(entity);

             foreach (var request in _showLineRequest.Consume())
             {
                 typing.line = request.line;
                 typing.visibleChars = 0f;
                 typing.isTyping = true;
                 typing.stayTime = 0f;
                 typing.active = true;
             }
        
             foreach (var _ in _skipRequest.Consume())
             {
                 if (!typing.active) continue;
        
                 if (typing.isTyping)
                 {
                     typing.visibleChars = LineLength(typing);
                     typing.isTyping = false;
                     typing.stayTime = _settings.dialogueStayTime;
                 }
                 else
                 {
                     FinishLine(ref typing);
                 }
             }
        
             if (!typing.active) return;
        
             if (typing.isTyping)
             {
                 typing.visibleChars += _settings.dialogueCharsPerSecond * deltaTime;
        
                 if (typing.visibleChars >= LineLength(typing))
                 {
                     typing.visibleChars = LineLength(typing);
                     typing.isTyping = false;
                     typing.stayTime = _settings.dialogueStayTime;
                 }
             }
             else
             {
                 typing.stayTime -= deltaTime;
        
                 if (typing.stayTime <= 0f)
                     FinishLine(ref typing);
             }
        }
        
        private void FinishLine(ref TypingComponent typing)
        {
            typing.active = false;
            _lineFinishedEvent.NextFrame(new TypingEndEvent());
        }
        
        private static int LineLength(in TypingComponent typing)
        {
            return typing.line.text?.Length ?? 0;
        }

        public void Dispose() { }
    }
}
