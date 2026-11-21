using System;
using System.Collections.Generic;
using Configs;
using Features.BattlePack;
using Features.ControlPointPack.Components;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.ManaPack;
using Features.PathFinderPack;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Global;
using Scellecs.Morpeh;
using Random = UnityEngine.Random;

namespace Features.ControlPointPack.Systems
{
    public class ActivateControlPointSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _controlPointFilter;
        private Filter _playerFilter;
        private Filter _gridFilter;
        private Filter _battleFilter;
        
        private Stash<BattleComponent> _battleStash;
        private Stash<GridComponent> _gridStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<ControlPointComponent> _controlPointStash;
        private Stash<ControlPointActivatedTag> _controlPointActivatedTag;

        private Event<PhaseChangedEvent> _phaseChangedEvent;
        
        private Request<ManaChangeRequest> _manaRequest;
        private Request<UnitSpawnRequest> _unitSpawnRequest;
        
        public void OnAwake()
        {
            _controlPointFilter = World.Filter
                .With<ControlPointComponent>()
                .Without<ControlPointActivatedTag>()
                .Build();
            
            _battleFilter = World.Filter.With<BattleComponent>().Build();
            _playerFilter = World.Filter.With<PlayerTag>().Build();
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _battleStash = World.GetStash<BattleComponent>();
            _gridStash = World.GetStash<GridComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _controlPointStash = World.GetStash<ControlPointComponent>();
            _controlPointActivatedTag = World.GetStash<ControlPointActivatedTag>();
            
            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
            
            _unitSpawnRequest = World.GetRequest<UnitSpawnRequest>();
            _manaRequest = World.GetRequest<ManaChangeRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var phaseEvent in _phaseChangedEvent.publishedChanges)
            {
                if (phaseEvent.phase != TurnPhase.Events || phaseEvent.type == TurnType.Start)
                    continue;
                
                foreach (var controlPoint in _controlPointFilter)
                {
                    ref var controlPointComp = ref _controlPointStash.Get(controlPoint);

                    if (controlPointComp.teamSide == TeamSide.None)
                        continue;
                
                    var battle = _battleFilter.First();
                    ref var battleComp = ref _battleStash.Get(battle);

                    var spawnUnits = battleComp.battlePlan.ControlPointSpawnUnits;
                    var manaReward = battleComp.battlePlan.ManaReward;
                
                    if (controlPointComp.teamSide == TeamSide.Player)
                    {
                        var player = _playerFilter.First();
                    
                        _manaRequest.Publish(new ManaChangeRequest(manaReward, player));
                    }
                    else if (controlPointComp.teamSide == TeamSide.Enemy)
                    {
                        var grid = _gridFilter.First();
                        ref var gridComp = ref _gridStash.Get(grid);

                        var spawnCoord = FindRelevantCell(gridComp, controlPointComp.cellCoord);
                        var config = spawnUnits[Random.Range(0, spawnUnits.Count)];

                        if (spawnCoord != default(CellCoord))
                        {
                            _unitSpawnRequest.Publish(new UnitSpawnRequest(spawnCoord, config, true, false));
                        }
                    }
                
                    _controlPointActivatedTag.Add(controlPoint);
                }
            }
        }

        private CellCoord FindRelevantCell(GridComponent gridComp, CellCoord from)
        {
            foreach (var dir in PathFinder.Directions)
            {
                var coord = new CellCoord(from.Q + dir.Q, from.R + dir.R);

                var hasUnit = GridOccupancy.TryGetUnit(gridComp, _unitStash, coord, out var unit);

                if (!hasUnit)
                    return coord;
            }

            return default;
        }

        public void Dispose()
        {
            
        }
    }
}
