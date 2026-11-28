using Features.BattlePack;
using Features.EnemyAIPack.Components;
using Features.EnemyAIPack.Requests;
using Global;
using Scellecs.Morpeh;

namespace Features.EnemyAIPack.Systems
{
    public class EnemyWeightsSystem : ISystem
    {
        public World World { get; set; }

        private Singletone<EnemyWeightsComponent> _weights;
        private Singletone<BattleComponent> _battle;
        
        private Stash<EnemyWeightsComponent> _enemyWeightsStash;
        
        private Request<ChangeWeightRequest> _changeWeightRequest;
        
        private Event<BattleEvent> _battleEvent;

        public void OnAwake()
        {
            _weights = new Singletone<EnemyWeightsComponent>(World);
            _battle = new Singletone<BattleComponent>(World);

            _enemyWeightsStash = World.GetStash<EnemyWeightsComponent>();
            
            _changeWeightRequest = World.GetRequest<ChangeWeightRequest>();
            
            _battleEvent = World.GetEvent<BattleEvent>();

            InitSystem();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _battleEvent.publishedChanges)
            {
                var config = _battle.Comp.battlePlan.EnemyConfig;

                _weights.Comp = new EnemyWeightsComponent(config);
            }
            
            foreach (var request in _changeWeightRequest.Consume())
            {
                ref var weightsComp = ref _weights.Comp;
                var amount = request.changeWeightType == ChangeWeightType.Increase 
                    ? weightsComp.increaseValues[request.type]  
                    : weightsComp.reduceValues[request.type];
                
                weightsComp.weights[request.type] += request.changeWeightType == ChangeWeightType.Increase ? amount : -amount;
            }
        }

        private void InitSystem()
        {
            var entity = World.CreateEntity();
            var config = _battle.Comp.battlePlan.EnemyConfig;
            
            _enemyWeightsStash.Add(entity, new EnemyWeightsComponent(config));
        }
    
        public void Dispose() {}
    }
}
