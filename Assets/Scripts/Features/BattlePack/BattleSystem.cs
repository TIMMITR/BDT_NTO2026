using Features.GamePack;
using Features.UnitPack;
using Scellecs.Morpeh;

namespace Features.BattlePack
{
    public sealed class BattleSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<BattleComponent> _battleStash;
        private Request<BattleRequest> _battleRequest; 
        private Request<UnitSpawnRequest> _unitSpawnRequest;
        private Event<BattleEvent> _battleEvent;
        
        private readonly GamePlan _gamePlan;

        public BattleSystem(GamePlan gamePlan)
        {
            _gamePlan = gamePlan;
        }
        
        public void OnAwake()
        {
            _filter = World.Filter.With<BattleComponent>().Build();
            _battleStash = World.GetStash<BattleComponent>();
            _battleRequest = World.GetRequest<BattleRequest>();
            _unitSpawnRequest =  World.GetRequest<UnitSpawnRequest>();
            
            ref var battleComponent = ref _battleStash.Add(World.CreateEntity());
            battleComponent.currentBattleId = -1; // пока что без боя
        }

        public void OnUpdate(float deltaTime)
        {
            // тут иницилизируется бой 
            if (_filter.IsEmpty()) return;
            
            foreach (var request in _battleRequest.Consume())
            {
                if (request.id < 0 || request.id > _gamePlan.BattlePlans.Count)
                    continue;
                
                var battlePlan = _gamePlan.BattlePlans[request.id];

                foreach (var tile in battlePlan.HexTiles)
                {
                    if (tile.unitConfig != null)
                    {
                        _unitSpawnRequest.Publish(new UnitSpawnRequest
                        {
                            q = tile.q,
                            r = tile.r,
                            unitConfig = tile.unitConfig
                        });
                    }

                    if (tile.blocked)
                    {
                        // todo чето делать с сеткой, это я оставлю тебе Кирилл
                    }
                }
                
                _battleEvent.NextFrame(new BattleEvent()); // начинается бой
                break;
            }
        }

        public void Dispose() { }
    }
}