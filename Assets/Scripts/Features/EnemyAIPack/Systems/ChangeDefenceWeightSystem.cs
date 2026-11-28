using Configs;
using Features.EnemyAIPack.Requests;
using Features.HealthPack;
using Global;
using Scellecs.Morpeh;

namespace Features.EnemyAIPack.Systems
{
    public class ChangeDefenceWeightSystem : ISystem // todo возможно, стоит объединить подобные системы вместе
    {
        public World World { get; set; }

        private Stash<BaseTag> _baseTag;
        
        private Request<ChangeWeightRequest> _changeWeightRequest;
        
        private Event<HealthChangedEvent> _healthChangedEvent;

        public void OnAwake()
        {
            _baseTag = World.GetStash<BaseTag>();
            
            _changeWeightRequest = World.GetRequest<ChangeWeightRequest>();
            
            _healthChangedEvent = World.GetEvent<HealthChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _healthChangedEvent.publishedChanges)
            {
                if (!_baseTag.Has(evt.entity))
                    return;
                
                _changeWeightRequest.Publish(new ChangeWeightRequest(EnemyAIAction.Defence, ChangeWeightType.Increase));
            }
        }
        
        public void Dispose() { }
    }
}
