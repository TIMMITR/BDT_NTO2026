using Features.ControlPointPack.Requests;
using Features.GamePack;
using Features.GridPack.Data;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;

namespace Features.BattlePack.Systems
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
            
            StartFirstBattle();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _battleRequest.Consume())
            {
                if (request.id < 0 || request.id > _gamePlan.BattleSets.Count)
                    continue;

                var battle = _battleFilter.First();
                var newBattle = _gamePlan.BattleSets[request.id];
                var plan = newBattle.battlePlan;
                var story = newBattle.storyTalkAfterBattle;
                
                _battleStash.Set(battle, new BattleComponent(plan, story, request.id));
                
                foreach (var tile in plan.Units)
                {
                    _unitSpawnRequest.Publish(new UnitSpawnRequest
                    (
                        new CellCoord(tile.q, tile.r),
                        tile.unitConfig,
                        tile.blocked,
                        false
                    ));
                }

                foreach (var controlPoint in plan.ControlPoints)
                {
                    _spawnControlPointRequest.Publish(new SpawnControlPointRequest
                    (
                        controlPoint.config,
                        new CellCoord(controlPoint.q, controlPoint.r)
                    ));
                }

                _unitSpawnRequest.Publish(new UnitSpawnRequest
                (
                    new CellCoord(plan.BaseCoordQ, plan.BaseCoordR),
                    plan.BaseConfig,
                    true,
                    true
                ));    
                
                _battleEvent.NextFrame(new BattleEvent()); // начинается бой
                
                break;
            }
        }

        private void StartFirstBattle()
        {
            var battle = _gamePlan.BattleSets[0];
            var plan = battle.battlePlan;
            var story = battle.storyTalkAfterBattle;
            
            _battleStash.Add(World.CreateEntity(), new BattleComponent(plan, story, 0));
            _battleRequest.Publish(new BattleRequest{id = 0}, allowNextFrame: true);
        }

        public void Dispose() { }
    }
}