using Features.ControlPointPack.Requests;
using Features.GamePack;
using Features.GridPack.Data;
using Features.UnitPack;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;

namespace Features.BattlePack
{
    public sealed class BattleSystem : ISystem
    {
        public World World { get; set; }

        private Filter _battleFilter;
        
        private Stash<BattleComponent> _battleStash;
        
        private Request<BattleRequest> _battleRequest; 
        private Request<UnitSpawnRequest> _unitSpawnRequest;
        private Request<SpawnControlPointRequest> _spawnControlPointRequest;
        
        private Event<BattleEvent> _battleEvent;
        
        private readonly GamePlan _gamePlan;

        public BattleSystem(GamePlan gamePlan)
        {
            _gamePlan = gamePlan;
        }
        
        public void OnAwake()
        {
            _battleFilter = World.Filter.With<BattleComponent>().Build();
            
            _battleStash = World.GetStash<BattleComponent>();
            
            _battleRequest = World.GetRequest<BattleRequest>();
            _unitSpawnRequest =  World.GetRequest<UnitSpawnRequest>();
            _spawnControlPointRequest = World.GetRequest<SpawnControlPointRequest>();
            
            _battleEvent = World.GetEvent<BattleEvent>();
            
            ref var battleComponent = ref _battleStash.Add(World.CreateEntity());
            battleComponent.currentBattleId = 0; 
            battleComponent.battlePlan = _gamePlan.BattleSets[0].battlePlan;
            
            _battleRequest.Publish(new BattleRequest{id = battleComponent.currentBattleId}, allowNextFrame: true);
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _battleRequest.Consume())
            {
                if (request.id < 0 || request.id > _gamePlan.BattleSets.Count)
                    continue;

                var battle = _battleFilter.First();
                ref var battleComp = ref _battleStash.Get(battle);
                
                var battlePlan = _gamePlan.BattleSets[request.id].battlePlan;

                battleComp.currentBattleId = request.id;
                battleComp.battlePlan = battlePlan;
                
                foreach (var tile in battlePlan.Units)
                {
                    _unitSpawnRequest.Publish(new UnitSpawnRequest
                    (
                        new CellCoord(tile.q, tile.r),
                        tile.unitConfig,
                        tile.blocked,
                        false
                    ));
                }

                foreach (var controlPoint in battlePlan.ControlPoints)
                {
                    _spawnControlPointRequest.Publish(new SpawnControlPointRequest
                    (
                        controlPoint.config,
                        new CellCoord(controlPoint.q, controlPoint.r)
                    ));
                }

                _unitSpawnRequest.Publish(new UnitSpawnRequest
                (
                    new CellCoord(battlePlan.BaseCoordQ, battlePlan.BaseCoordR),
                    battlePlan.BaseConfig,
                    true,
                    true
                ));    
                
                _battleEvent.NextFrame(new BattleEvent()); // начинается бой
                
                break;
            }
        }

        public void Dispose() { }
    }
}