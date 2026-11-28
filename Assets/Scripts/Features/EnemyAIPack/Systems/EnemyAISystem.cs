using System.Collections.Generic;
using Configs;
using Features.ControlPointPack.Components;
using Features.EnemyAIPack.Components;
using Features.EnemyAIPack.Requests;
using Features.FightPack.Components;
using Features.GridPack.Components;
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

namespace Features.EnemyAIPack.Systems
{
    public class EnemyAISystem : ISystem
    {
        public World World { get; set; }

        private Filter _unitFilter;
        private Filter _controlPointFilter;
        private Filter _movingFilter;
        private Filter _baseFilter;

        private Singletone<GridComponent> _grid;
        private Singletone<EnemyWeightsComponent> _weights;
        
        private Stash<CellPositionComponent> _positionStash;
        private Stash<CellTileComponent> _cellTileStash;
        private Stash<ControlPointComponent> _controlPointStash;
        private Stash<SpeedComponent> _speedStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<SpellDataComponent> _spellStash;
        private Stash<AttackDesireComponent> _attackDesireStash;

        private Stash<BusyTag> _busyTag;
        private Stash<SkipTag> _skipTagStash;
        
        private Event<PhaseChangedEvent> _phaseChangedEvent;

        private Request<EndEnemyTurnRequest> _endTurnRequest;
        private Request<MoveUnitRequest> _moveUnitRequest;
        private Request<ChangeWeightRequest> _changeWeightRequest;

        private List<Entity> _units = new();
        
        private EnemyContext _context;
        
        private bool _isActive;
        private int _currentIndex;

        public void OnAwake()
        {
            _unitFilter = World.Filter.With<UnitComponent>().Build();
            _controlPointFilter = World.Filter.With<ControlPointComponent>().Build();
            _movingFilter = World.Filter.With<BusyTag>().Build();
            _baseFilter = World.Filter.With<BaseTag>().Build();

            _grid = new Singletone<GridComponent>(World);
            _weights = new Singletone<EnemyWeightsComponent>(World);
            
            _positionStash = World.GetStash<CellPositionComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            _controlPointStash = World.GetStash<ControlPointComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _spellStash = World.GetStash<SpellDataComponent>();
            _attackDesireStash = World.GetStash<AttackDesireComponent>();

            _busyTag = World.GetStash<BusyTag>();
            _skipTagStash = World.GetStash<SkipTag>();
            
            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
            _moveUnitRequest = World.GetRequest<MoveUnitRequest>();
            _endTurnRequest = World.GetRequest<EndEnemyTurnRequest>();
            _changeWeightRequest = World.GetRequest<ChangeWeightRequest>();

            InitContext();
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
                _isActive = false;
                _units.Clear();
                _endTurnRequest.Publish(new EndEnemyTurnRequest(), allowNextFrame: true);
                return;
            }
 
            var unit = _units[_currentIndex];

            if (_skipTagStash.Has(unit))
            {
                _skipTagStash.Remove(unit);
                _currentIndex++;
                return;
            }
 
            if (_busyTag.Has(unit))
                return;
 
            if (!_movingFilter.IsEmpty())
                return;
 
            Iterate(unit);
 
            _currentIndex++;
        }
        
        private void Iterate(Entity unit)
        {
            _context.unit = unit;

            var strategies = _weights.Comp.weights;
            
            EnemyAIAction? chosen = null;
            var bestChance = float.MinValue;
 
            foreach (var pair in strategies)
            {
                var chance = (1f + Random.value) * pair.Value;
 
                if (chance > bestChance)
                {
                    bestChance = chance;
                    chosen = pair.Key;
                }
            }
 
            if (chosen == null)
                return;

            _changeWeightRequest.Publish(new ChangeWeightRequest(chosen.Value, ChangeWeightType.Decrease));
        }
        
        private void BeginPhase()
        {
            _units.Clear();
            _currentIndex = 0;
 
            foreach (var unit in _unitFilter)
                if (_unitStash.Get(unit).teamSide == TeamSide.Enemy)
                    _units.Add(unit);
 
            _isActive = true;
        }

        private void InitContext()
        {
            _context = new EnemyContext();
            
            _context.unitFilter = _unitFilter;
            _context.controlPointFilter = _controlPointFilter;
            _context.baseFilter =  _baseFilter;
            
            _context.grid = _grid;
            
            _context.positionStash = _positionStash;
            _context.cellTileStash = _cellTileStash;
            _context.controlPointStash = _controlPointStash;
            _context.speedStash = _speedStash;
            _context.unitStash = _unitStash;
            _context.spellStash = _spellStash;
            _context.attackDesireStash = _attackDesireStash;
            
            _context.moveRequest = _moveUnitRequest;
        }

        public void Dispose() { }
    }
}