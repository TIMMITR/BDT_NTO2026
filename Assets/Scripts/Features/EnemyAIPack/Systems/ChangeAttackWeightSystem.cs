using Configs;
using Features.EnemyAIPack.Requests;
using Features.HealthPack;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.EnemyAIPack.Systems
{
    public class ChangeAttackWeightSystem : ISystem
    {
        public World World { get; set; }

        private Stash<UnitComponent> _unitStash;
        
        private Request<ChangeWeightRequest> _changeWeightRequest;
        
        private Event<HealthChangedEvent> _healthChangedEvent;

        public void OnAwake()
        {
            _unitStash = World.GetStash<UnitComponent>();
            
            _changeWeightRequest = World.GetRequest<ChangeWeightRequest>();
            
            _healthChangedEvent = World.GetEvent<HealthChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _healthChangedEvent.publishedChanges)
            {
                if (!_unitStash.Has(evt.entity))
                    continue;
                
                ref var unitComp = ref _unitStash.Get(evt.entity); // пока что логика такая: кто-то атаковал игрока -> больше юнитов пойдут его атаковать

                if (unitComp.teamSide != TeamSide.Player) 
                    continue;
                
                _changeWeightRequest.Publish(new ChangeWeightRequest(EnemyAIAction.Attack, ChangeWeightType.Increase));
            }
        }
        
        public void Dispose() { }
    }
}
