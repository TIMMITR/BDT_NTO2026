using Configs;
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
        

        public void OnAwake()
        {
            _request = World.GetRequest<PlayEffectRequest>();
            _unitMovedEvent = World.GetEvent<UnitMovedEvent>();
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
        }

        public void Dispose() { }
    }
}