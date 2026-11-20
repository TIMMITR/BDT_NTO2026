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
            _battleStash = World.GetStash<BattleComponent>();
            
            _battleRequest = World.GetRequest<BattleRequest>();
            _unitSpawnRequest =  World.GetRequest<UnitSpawnRequest>();
            _spawnControlPointRequest = World.GetRequest<SpawnControlPointRequest>();
            
            _battleEvent = World.GetEvent<BattleEvent>();
            
            ref var battleComponent = ref _battleStash.Add(World.CreateEntity());
            battleComponent.currentBattleId = 0; 
            
            _battleRequest.Publish(new BattleRequest{id = battleComponent.currentBattleId}, allowNextFrame: true);
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _battleRequest.Consume())
            {
                if (request.id < 0 || request.id > _gamePlan.BattlePlans.Count)
                    continue;
                
                var battlePlan = _gamePlan.BattlePlans[request.id];

                foreach (var tile in battlePlan.Units)
                {
                    _unitSpawnRequest.Publish(new UnitSpawnRequest
                    (
                        tile.q, tile.r,
                        tile.unitConfig,
                        tile.blocked
                    ));
                }

                foreach (var controlPoint in battlePlan.Controlpoints)
                {
                    _spawnControlPointRequest.Publish(new SpawnControlPointRequest
                    (
                        controlPoint.config,
                        new CellCoord(controlPoint.q, controlPoint.r)
                    ));
                }
                
                _battleEvent.NextFrame(new BattleEvent()); // начинается бой
                
                break;
            }
        }

        public void Dispose() { }
    }
}