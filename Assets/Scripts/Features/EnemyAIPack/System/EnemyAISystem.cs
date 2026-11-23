using System.Collections.Generic;
using Configs;
using Features.BattlePack;
using Features.CalcPack;
using Features.CellSelectionPack.Components;
using Features.ControlPointPack.Components;
using Features.FightPack.Components;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PathFinderPack;
using Features.SpeedPack;
using Features.SpellPack.Components;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Features.StatePack.Requests;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Global;
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
        private Filter _battleFilter;
        private Filter _movingFilter;
        private Filter _baseFilter;
        
        private Stash<GridComponent> _gridStash;
        private Stash<CellPositionComponent> _positionStash;
        private Stash<CellTileComponent> _cellTileStash;
        private Stash<ControlPointComponent> _controlPointStash;
        private Stash<SpeedComponent> _speedStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<SpellDataComponent> _spellStash;
        private Stash<AttackDataComponent> _attackStash;
        private Stash<DelayAttackComponent> _delayAttackStash;
        private Stash<BattleComponent> _battleStash;
        private Stash<UnitAnimationComponent> _unitAnimationStash;
        
        private Event<PhaseChangedEvent> _phaseChangedEvent;

        private Request<EndEnemyTurnRequest> _endTurnRequest;
        private Request<MoveUnitRequest> _moveUnitRequest;
        
        private EnemyConfigSo _config;

        private List<Entity> _units = new();
        
        private float _attackWeight;
        private float _captureWeight;
        private float _defenceWeight;
        
        private bool _isActive;
        private int _currentIndex;

        public void OnAwake()
        {
            _unitFilter = World.Filter.With<UnitComponent>().Build();
            _gridFilter = World.Filter.With<GridComponent>().Build();
            _controlPointFilter = World.Filter.With<ControlPointComponent>().Build();
            _battleFilter = World.Filter.With<BattleComponent>().Build();
            _movingFilter = World.Filter.With<UnitAnimationComponent>().Build();
            _baseFilter = World.Filter.With<BaseTag>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            _controlPointStash = World.GetStash<ControlPointComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _spellStash = World.GetStash<SpellDataComponent>();
            _attackStash = World.GetStash<AttackDataComponent>();
            _delayAttackStash = World.GetStash<DelayAttackComponent>();
            _battleStash = World.GetStash<BattleComponent>();
            _unitAnimationStash = World.GetStash<UnitAnimationComponent>();
            
            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
            _moveUnitRequest = World.GetRequest<MoveUnitRequest>();
            _endTurnRequest = World.GetRequest<EndEnemyTurnRequest>();
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
                _isActive = false;
                _units.Clear();
                _endTurnRequest.Publish(new EndEnemyTurnRequest(), allowNextFrame: true);
                return;
            }
            
            var unit = _units[_currentIndex];

            if (_unitAnimationStash.Has(unit) || _delayAttackStash.Has(unit))
                return;

            if (!_movingFilter.IsEmpty())
                return;
            
            Iterate(unit);
            
            _currentIndex++;
        }

        private void Iterate(Entity unit)
        {
            var gridEntity = _gridFilter.First();
            ref var gridComp = ref _gridStash.Get(gridEntity);
            var from = _positionStash.Get(unit).coord;

            var attackChance = (1 + Random.value) * _attackWeight;
            var captureChance = (1 + Random.value) * _captureWeight;
            var defenceChance = (1 + Random.value) * _defenceWeight;

            var maxChance = Mathf.Max(attackChance, captureChance, defenceChance); // todo надо будет при добавлении доп весов

            if (Mathf.Approximately(captureChance, maxChance) && TryCapture(gridComp, from, unit)) 
                // для избежания погрешности float
            {
                _captureWeight -= _config.CaptureReduceValue;
                Debug.Log($"Enemy has chosen to go to the control point. Current capture weight: {_captureWeight}");
            }
            else if (Mathf.Approximately(attackChance, maxChance) && TryAttack(gridComp, gridEntity, from, unit)) // todo избавиться от первого условия (система весов не работает)
            {
                _attackWeight -= _config.AttackReduceValue;
                Debug.Log($"Enemy has chosen to go to the player unit. Current attack weight: {_attackWeight}");
            }
            else if (Mathf.Approximately(defenceChance, maxChance) && TryDefend(gridComp, from, unit))
            {
                _defenceWeight -= _config.DefenceReduceValue;
                Debug.Log($"Enemy has chosen to defend the base. Current defence weight: {_defenceWeight}");
            }
        }

        private bool TryCapture(GridComponent gridComp, CellCoord from, Entity unit)
        {
            var speed = _speedStash.Get(unit).currentSpeed;
            
            if (!TryGetPathToPoint(gridComp, from, speed, out var resultPath))
                return false;
            
            _moveUnitRequest.Publish(new MoveUnitRequest(unit, resultPath), allowNextFrame: true); 
            return true;
        }

        private bool TryAttack(GridComponent gridComp, Entity gridEntity, CellCoord from, Entity unit)
        {
            if (!TryFindPlayer(gridComp, from, out var target))
                return false;
            
            if (!_spellStash.Has(unit))
                return false;
            
            var spell = _spellStash.Get(unit).spellData;

            if (from.DistanceTo(target) <= spell.range)
            {
                Attack(gridEntity, unit, spell, from, target);
                return true;
            }

            var newPos = MoveToTarget(gridComp, from, target, unit);
            
            if(newPos.DistanceTo(target) <= spell.range)
                Attack(gridEntity, unit, spell, from, target);

            return true;
        }
        
        private bool TryDefend(GridComponent gridComp, CellCoord from, Entity unit)
        {
            var baseCoord = _positionStash.Get(_baseFilter.First()).coord;
            var radius = _config.DefenceRadius;

            if (from.DistanceTo(baseCoord) <= radius)
                return true;

            var speed = _speedStash.Get(unit).currentSpeed;
            if (speed <= 0) 
                return false;

            List<CellCoord> resultPath = null;
            bool gotRingCells = TryGetRingCells(gridComp, baseCoord, radius, out var ringCells);

            if (!gotRingCells)
                return false;
            
            foreach (var cell in ringCells)
            {
                if (!GridData.IsFreeCell(gridComp, _cellTileStash, cell)) continue;

                var path = BuildPath(gridComp, from, cell, speed);
                if (path != null && (resultPath == null || path.Count < resultPath.Count))
                    resultPath = path;
            }

            if (resultPath == null)
                return false;
            
            _moveUnitRequest.Publish(new MoveUnitRequest(unit, resultPath), allowNextFrame: true);
            
            return true;
        }
        
        private bool TryGetRingCells(GridComponent gridComp, CellCoord center, int radius, out List<CellCoord> resultPath)
        {
            resultPath = new List<CellCoord>();
            
            foreach (var coord in gridComp.cells.Keys)
            {
                if (center.DistanceTo(coord) != radius) 
                    continue;
                if (GridData.IsFreeCell(gridComp, _cellTileStash, coord))
                    resultPath.Add(coord);
            }
            
            return resultPath.Count >= 0;
        }

        private void Attack(Entity gridEntity, Entity unit, SpellConfigSo spell, CellCoord from, CellCoord target)
        {
            _attackStash.Set(gridEntity, new AttackDataComponent(spell, from, target));
            _delayAttackStash.Add(unit, new DelayAttackComponent(unit));
        }

        private bool TryFindPlayer(GridComponent gridComp, CellCoord from, out CellCoord target)
        {
            var champion = int.MaxValue;
            target = default;

            foreach (var unit in _unitFilter)
            {
                if (_unitStash.Get(unit).teamSide != TeamSide.Player)
                    continue;
                
                var coord = _positionStash.Get(unit).coord;
                var dist = from.DistanceTo(coord);

                if (dist < champion)
                {
                    champion = dist;
                    target = coord;
                }
            }

            return champion < int.MaxValue;
        }

        private CellCoord MoveToTarget(GridComponent gridComp, CellCoord from, CellCoord targetCoord, Entity unit)
        {
            var speed = _speedStash.Get(unit).currentSpeed;

            if (speed <= 0)
                return from;

            List<CellCoord> resultPath = null;

            foreach (var dir in HexMath.Directions)
            {
                var next = new CellCoord(targetCoord.Q + dir.Q, targetCoord.R + dir.R);
                if (!GridData.IsFreeCell(gridComp, _cellTileStash, next))
                    continue;

                var path = BuildPath(gridComp, from, next, speed);
                    
                if (path != null && ( resultPath == null || path.Count < resultPath.Count))
                    resultPath = path;
            }

            if (resultPath == null)
                return from;
            
            _moveUnitRequest.Publish(new MoveUnitRequest(unit, resultPath), allowNextFrame: true); // todo возможно, стоит избавиться от allowNextFrame
            return resultPath[^1];
        }

        private bool TryGetPathToPoint(GridComponent gridComp, CellCoord from, int speed, out List<CellCoord> resultPath)
        {
            resultPath = null;
            
            if (speed <= 0)
                return false;

            foreach (var controlPoint in _controlPointFilter)
            {
                ref var controlPointComp = ref _controlPointStash.Get(controlPoint);
                var coord = controlPointComp.cellCoord;

                foreach (var dir in HexMath.Directions)
                {
                    var next = new CellCoord(coord.Q + dir.Q, coord.R + dir.R);
                    if (!GridData.IsFreeCell(gridComp, _cellTileStash, next))
                        continue;

                    var path = BuildPath(gridComp, from, next, speed);
                    
                    if (path != null && (resultPath == null || path.Count < resultPath.Count))
                        resultPath = path;
                }
            }
            
            return resultPath != null;
        }

        private List<CellCoord> BuildPath(GridComponent gridComp, CellCoord from, CellCoord to, int speed)
        {
            var ready = PathFinder.GetPath(gridComp, from, to, _cellTileStash);

            if (!ready.path.ContainsKey(to))
                return null;
            
            var path = PathFinder.ReconstructPath(ready, from, to);

            if (path.Count <= 1)
                return null;
            
            path.Insert(0, from);
            
            while (path.Count - 1 > speed)
                path.RemoveAt(path.Count - 1);
            
            return path;
        }

        private void BeginPhase()
        {
            _units.Clear();
            _currentIndex = 0;
            
            foreach (var unit in _unitFilter)
                if (_unitStash.Get(unit).teamSide == TeamSide.Enemy)
                    _units.Add(unit);
            
            var battle = _battleFilter.First();
            ref var battleComp = ref _battleStash.Get(battle);
            
            _config = battleComp.battlePlan.EnemyConfig;

            _attackWeight = _config.AttackWeight;
            _captureWeight = _config.CaptureWeight;
            _defenceWeight = _config.DefenceWeight;
            
            _isActive = true;
        }

        public void Dispose() { }
    }
}
