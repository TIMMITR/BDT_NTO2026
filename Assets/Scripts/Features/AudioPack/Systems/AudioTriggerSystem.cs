using Configs;
using Features.TypingPack.Data;
using Features.TypingPack.Events;
using Features.UnitPack.Events;
using Scellecs.Morpeh;

namespace Features.AudioPack.Systems
{
    public sealed class AudioTriggerSystem : ISystem
    {
        public World World { get; set; }
        
        // слушает ивенты и кидает реквесты
        // ух сдесь много чего будет....
        
        private Request<PlayEffectRequest> _request;
        private Event<UnitMovedEvent> _unitMovedEvent;
        private Event<SymbolTypedEvent> _typingEvent;
        

        public void OnAwake()
        {
            _request = World.GetRequest<PlayEffectRequest>();
            _unitMovedEvent = World.GetEvent<UnitMovedEvent>();
            _typingEvent = World.GetEvent<SymbolTypedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var _ in _unitMovedEvent.publishedChanges)
            {
                _request.Publish(new PlayEffectRequest
                {
                    audioType = AudioType.UnitMove
                });
            }

            foreach (var evt in _typingEvent.publishedChanges)
            {
                _request.Publish(new PlayEffectRequest
                {
                    audioType = evt.speaker == Speaker.Warlock
                        ? AudioType.WarlockVoice
                        : AudioType.VireteVoice
                });
            }
        }

        public void Dispose() { }
    }
}