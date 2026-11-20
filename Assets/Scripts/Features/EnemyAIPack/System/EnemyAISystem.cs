using System.Collections.Generic;
using Configs;
using Features.ControlPointPack.Components;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PathFinderPack;
using Features.SpeedPack;
using Features.SpeedPack.Requests;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Features.StatePack.Requests;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.EnemyAIPack.System
{
    public class EnemyAISystem : ISystem
    {
        public World World { get; set; }

        private Filter _unitFilter;
        private Filter _gridFilter;
        private Filter _controlPointFilter;
        
        private Stash<GridComponent> _gridStash;
        private Stash<CellPositionComponent> _positionStash;
        private Stash<CellTileComponent> _cellTileStash;
        private Stash<ControlPointComponent> _controlPointStash;
        private Stash<SpeedComponent> _speedStash;
        
        private Event<PhaseChangedEvent> _phaseChangedEvent;

        private Request<EndEnemyTurnRequest> _endTurnRequest;
        private Request<MoveUnitRequest> _moveUnitRequest;
        
        private readonly EnemyConfigSo _config;

        private List<Entity> _units;
        
        private float _attackWeight;
        private float _captureWeight;
        
        private bool _isActive;
        private int _currentIndex;

        private bool IsFreeCell(GridComponent gridComp, CellCoord coord) => // todo надо определиться где писать свойства
            gridComp.cells.TryGetValue(coord, out var cell)
            && !gridComp.takenCells.ContainsKey(coord)
            && !_cellTileStash.Get(cell).blocked;

        public EnemyAISystem(EnemyConfigSo config)
        {
            _config = config;
            
            _units = new List<Entity>();
        }

        public void OnAwake()
        {
            _unitFilter = World.Filter.With<UnitComponent>().Build();
            _gridFilter = World.Filter.With<GridComponent>().Build();
            _controlPointFilter = World.Filter.With<ControlPointComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            _controlPointStash = World.GetStash<ControlPointComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            
            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
            _moveUnitRequest = World.GetRequest<MoveUnitRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _phaseChangedEvent.publishedChanges)
                if (evt.phase == TurnPhase.Enemy && evt.type == TurnType.Start)
                    BeginPhase();

            if (!_isActive)
                return;

            if (_units.Count == 0 || _currentIndex >= _units.Count)
            {
                Debug.Log("Все юниты уничтожены");
                _endTurnRequest.Publish(new EndEnemyTurnRequest());
                return;
            }
            
            var unit = _units[_currentIndex];
            
            Iterate(unit);
            _currentIndex++;
        }

        private void Iterate(Entity unit)
        {
            var gridEntity = _gridFilter.First();
            ref var gridComp = ref _gridStash.Get(gridEntity);
            var from = _positionStash.Get(gridEntity).coord;

            var attackChance = (1 + Random.value) * _attackWeight;
            var captureChance = (1 + Random.value) * _captureWeight;

            var maxChance = Mathf.Max(attackChance, captureChance); // todo надо будет при добавлении доп весов

            if (Mathf.Approximately(attackChance, maxChance)) // для избежания погрешности float
            {
                TryCapture(gridComp, from, unit);
            }

        }

        private bool TryCapture(GridComponent gridComp, CellCoord from, Entity unit)
        {
            var speed = _speedStash.Get(unit).currentSpeed;
            
            if (!TryGetPathToPoint(gridComp, from, speed, out var resultPath))
                return false;
            
            _moveUnitRequest.Publish(new MoveUnitRequest(unit, resultPath));
            return true;
        }

        private bool TryGetPathToPoint(GridComponent gridComp, CellCoord from, int speed, out List<CellCoord> resultPath)
        {
            resultPath = new List<CellCoord>();
            
            if (speed <= 0)
                return false;

            foreach (var controlPoint in _controlPointFilter)
            {
                ref var controlPointComp = ref _controlPointStash.Get(controlPoint);
                var coord = controlPointComp.cellCoord;

                foreach (var dir in PathFinder.Directions)
                {
                    var next = new CellCoord(coord.Q + dir.Q, coord.R + dir.R);
                    if (!IsFreeCell(gridComp, next))
                        continue;

                    var path = BuildPath(gridComp, from, next, speed);
                    
                    if (path.Count < resultPath.Count)
                        resultPath = path;
                }
            }
            
            return resultPath.Count > 0;
        }

        private List<CellCoord> BuildPath(GridComponent gridComp, CellCoord from, CellCoord to, int speed)
        {
            var ready = PathFinder.GetPath(gridComp, from, to, _cellTileStash);
            
            var path = PathFinder.ReconstructPath(ready, from, to);

            if (path.Count <= 1)
                return null;
            
            while (path.Count - 1 > speed)
                path.RemoveAt(path.Count - 1);
            
            return path;
        }

        private void BeginPhase()
        {
            foreach (var unit in _unitFilter)
            {
                _units.Add(unit);
            }

            _attackWeight = _config.AttackWeight;
            _captureWeight = _config.CaptureWeight;
            
            _isActive = true;
        }

        public void Dispose() { }
    }
}
