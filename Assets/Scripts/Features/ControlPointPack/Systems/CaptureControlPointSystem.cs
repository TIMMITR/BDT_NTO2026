using Configs;
using Features.CalcPack;
using Features.ControlPointPack.Components;
using Features.ControlPointPack.Events;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PathFinderPack;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.ControlPointPack.Systems
{
    public class CaptureControlPointSystem : ISystem // todo надо добавить тег запрещающий заканчивать ход во время некоторых действия !!!!!!!!!!!
    {
        public World World { get; set; }

        private Filter _controlPointFilter;
        private Filter _gridFilter;

        private Stash<ControlPointComponent> _controlPointStash;
        private Stash<GridComponent> _gridStash;
        private Stash<UnitComponent> _unitStash;
        
        private Event<PhaseChangedEvent> _phaseChangedEvent;
        private Event<ControlPointChangedEvent> _controlPointChangedEvent;
        
        public void OnAwake()
        {
            _controlPointFilter = World.Filter.With<ControlPointComponent>().Build();
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _controlPointStash = World.GetStash<ControlPointComponent>();
            _gridStash = World.GetStash<GridComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            
            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
            _controlPointChangedEvent = World.GetEvent<ControlPointChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _phaseChangedEvent.publishedChanges)
            {
                if (evt.phase == TurnPhase.Events)
                    continue;
                
                var gridEntity = _gridFilter.First();   
                ref var gridComp = ref _gridStash.Get(gridEntity);
                
                foreach (var controlPoint in _controlPointFilter)
                {
                    var hasPlayer = false;
                    var hasEnemy = false;
                    
                    ref var controlPointComp = ref _controlPointStash.Get(controlPoint);
                    var coord = controlPointComp.cellCoord;
            
                    foreach (var neighbour in HexMath.GetNeighbours(coord))
                    {
                        if (!GridData.TryGetUnit(gridComp, _unitStash, neighbour, out var unit, out var side))
                            continue;

                        if(side == TeamSide.Player)
                            hasPlayer = true;
                        else if (side == TeamSide.Enemy)
                            hasEnemy = true;
                    }
                    
                    if (hasPlayer == hasEnemy)
                        continue;

                    controlPointComp.teamSide = hasPlayer ? TeamSide.Player : TeamSide.Enemy;
                    
                    _controlPointChangedEvent.NextFrame(new ControlPointChangedEvent(controlPoint));
                    Debug.Log($"Control point captured, coord: {coord}.");
                }
            }
        }
        
        public void Dispose() {}
    }
}
