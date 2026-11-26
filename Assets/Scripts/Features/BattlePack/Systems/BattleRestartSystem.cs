using Features.CellSelectionPack.Requests;
using Features.ControlPointPack.Components;
using Features.ManaPack;
using Features.SpeedPack.Requests;
using Features.StatePack.Requests;
using Features.UnitPack.Components;
using Global;
using Scellecs.Morpeh;

namespace Features.BattlePack.Systems
{
    public class BattleRestartSystem : ISystem
    {
        public World World { get; set; }

        private Filter _unitFilter;
        private Filter _controlPointFilter;
        private Filter _baseFilter;
        private Filter _battleFilter;
        
        private Stash<DeadTag> _deadStash;
        private Stash<DestroyTag> _destroyStash;
        private Stash<BattleComponent> _battleStash;
        
        private Request<BattleRequest> _battleRequest;
        private Request<StartPlayerTurnRequest> _startPlayerTurnRequest;
        private Request<DeselectUnitRequest> _deselectRequest;
        private Request<ClearColorsRequest> _clearColorsRequest;

        private bool _isRestarting; // todo возможно стоит убрать

        public void OnAwake()
        {
            _unitFilter = World.Filter.With<UnitComponent>().Build();
            _controlPointFilter = World.Filter.With<ControlPointComponent>().Build();
            _battleFilter = World.Filter.With<BattleComponent>().Build();
            
            _baseFilter = World.Filter.
                With<BaseTag>().
                With<DeadTag>().
                Build();
            
            _deadStash = World.GetStash<DeadTag>();
            _destroyStash = World.GetStash<DestroyTag>();
            _battleStash = World.GetStash<BattleComponent>();
            
            _battleRequest = World.GetRequest<BattleRequest>();
            _startPlayerTurnRequest = World.GetRequest<StartPlayerTurnRequest>();
            _deselectRequest = World.GetRequest<DeselectUnitRequest>();
            _clearColorsRequest = World.GetRequest<ClearColorsRequest>();
            
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_baseFilter.IsEmpty() && !_isRestarting)
            {
                foreach (var unit in _unitFilter)
                {
                    _deadStash.Add(unit);
                    _destroyStash.Add(unit);
                }

                foreach (var cp in _controlPointFilter)
                {
                    _deadStash.Add(cp);
                    _destroyStash.Add(cp);
                }
            
                _isRestarting = true;
            }

            if (!_controlPointFilter.IsEmpty() || !_unitFilter.IsEmpty())
                return;
            
            _deselectRequest.Publish(new DeselectUnitRequest());
            _clearColorsRequest.Publish(new ClearColorsRequest());
            _startPlayerTurnRequest.Publish(new StartPlayerTurnRequest());
 
            ref var battle = ref _battleStash.Get(_battleFilter.First());
            _battleRequest.Publish(new BattleRequest { id = battle.currentBattleId }, allowNextFrame: true);
        }
        
        public void Dispose()
        {
            
        }
    }
}
