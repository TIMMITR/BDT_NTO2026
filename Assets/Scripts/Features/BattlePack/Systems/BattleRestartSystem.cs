using Features.CellSelectionPack.Requests;
using Features.ControlPointPack.Components;
using Features.ManaPack;
using Features.SpeedPack.Requests;
using Features.StatePack.Requests;
using Features.StoryTalkPack.Events;
using Features.StoryTalkPack.Requests;
using Features.UnitPack.Components;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

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
        private Request<StoryTalkShowRequest> _storyTalkShowRequest;

        private Event<StoryTalkFinishEvent> _storyTalkFinishEvent;

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
            _storyTalkShowRequest = World.GetRequest<StoryTalkShowRequest>();
            
            _storyTalkFinishEvent = World.GetEvent<StoryTalkFinishEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_baseFilter.IsEmpty() && !_isRestarting)
            {
                foreach (var unit in _unitFilter)
                {
                    if(!_deadStash.Has(unit))
                        _deadStash.Add(unit);
                    if(!_destroyStash.Has(unit))
                        _destroyStash.Add(unit);
                }

                foreach (var cp in _controlPointFilter)
                {
                    _deadStash.Add(cp);
                    _destroyStash.Add(cp);
                }
            
                _isRestarting = true;
                
                ref var battleComp  = ref _battleStash.Get(_battleFilter.First());
                _storyTalkShowRequest.Publish(new StoryTalkShowRequest(battleComp.endStoryTalk));
                return;
            }

            
            if (!_controlPointFilter.IsEmpty() || !_unitFilter.IsEmpty())
                return;

            foreach (var evt in _storyTalkFinishEvent.publishedChanges)
            {
                _isRestarting = false;
            
                _deselectRequest.Publish(new DeselectUnitRequest(), allowNextFrame: true);
                _clearColorsRequest.Publish(new ClearColorsRequest(), allowNextFrame: true);
                _startPlayerTurnRequest.Publish(new StartPlayerTurnRequest(), allowNextFrame: true);
 
                ref var battle = ref _battleStash.Get(_battleFilter.First());
                
                Debug.Log(battle.currentBattleId + 1);
                _battleRequest.Publish(new BattleRequest { id = battle.currentBattleId + 1}, allowNextFrame: true);
            }
        }
        
        public void Dispose() {}
    }
}
