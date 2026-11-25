using System.Collections.Generic;
using Configs;
using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Requests;
using Features.CellSelectionPack.Tags;
using Features.CellTrackPack.Events;
using Features.FightPack;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PathFinderPack;
using Features.SpeedPack;
using Features.SpellPack.Components;
using Features.UnitPack.Components;
using Global;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Systems
{
    public class PathPreviewSystem : ISystem // todo этот скрипт надо зарефакторить, он выполняет несколько действий одновременно
    {
        public World World { get; set; }

        private Singletone<GridComponent> _grid;
        private Singletone<PlayerSelectionTag> _selection;
        
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<CellTileComponent> _cellTileStash;
        private Stash<CellPositionComponent> _positionStash;
        private Stash<SpellDataComponent> _spellDataStash;
        private Stash<PlannedPathComponent> _plannedPathStash;
        private Stash<PlannedRangeComponent> _plannedRangeStash;
        private Stash<PlannedAttackComponent> _plannedAttackStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<SpeedComponent> _speedStash;
        
        private Event<CellChooseChangedEvent> _cellChooseChangedEvent;
        
        private Request<PlanCalcRequest> _planCalcRequest; 
        private Request<RefreshHighlightRequest> _refreshHighlightRequest;

        public void OnAwake()
        {
            _grid = new Singletone<GridComponent>(World);
            _selection = new Singletone<PlayerSelectionTag>(World);
            
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            _spellDataStash = World.GetStash<SpellDataComponent>();
            _plannedPathStash = World.GetStash<PlannedPathComponent>();
            _plannedRangeStash = World.GetStash<PlannedRangeComponent>();
            _plannedAttackStash = World.GetStash<PlannedAttackComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            
            _cellChooseChangedEvent = World.GetEvent<CellChooseChangedEvent>();
            
            _planCalcRequest = World.GetRequest<PlanCalcRequest>();
            _refreshHighlightRequest = World.GetRequest<RefreshHighlightRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            bool hasToRecalculate = false;

            foreach (var _ in _cellChooseChangedEvent.publishedChanges)
                hasToRecalculate = true;
            foreach (var _ in _planCalcRequest.Consume())
                hasToRecalculate = true;

            if (!hasToRecalculate)
                return;

            var selected = _selection.Entity;

            if (_plannedAttackStash.Has(selected))
                _plannedAttackStash.Remove(selected);
            if (_plannedPathStash.Has(selected))
                _plannedPathStash.Remove(selected);
            if (_plannedRangeStash.Has(selected))
                _plannedRangeStash.Remove(selected);

            _refreshHighlightRequest.Publish(new RefreshHighlightRequest());

            if (!_selectedUnitStash.Has(selected) || !_chosenCellStash.Has(selected))
                return;
            
            var unit = _selectedUnitStash.Get(selected).unit;
            
            var from = _positionStash.Get(unit).coord;
            var to = _chosenCellStash.Get(selected).cellCoord; 
            
            if (from == to || GridData.GetUnitTeam(_grid.Comp, _unitStash, to) == TeamSide.Player)
                return;
 
            var speed = _speedStash.Get(unit).currentSpeed;
 
            if (_spellDataStash.Has(unit))
                BuildAttackPath(_spellDataStash.Get(unit).spellData, from, to, speed);
            else
                BuildMovePath(from, to, speed);
        }

        private void BuildAttackPath(SpellConfigSo spell, CellCoord from, CellCoord to, int speed)
        {
            var ready = PathFinder.GetPath(_grid.Comp, from, to, _cellTileStash);
 
            if (!AttackPlanner.TryFindAttackPos(_grid.Comp, _cellTileStash, ready, spell, to, speed,
                    out var position, out var dir))
                return;
 
            var path = new List<CellCoord> { from };
 
            if (position != from)
                path.AddRange(PathFinder.ReconstructPath(ready, from, position));
 
            var cells = new List<CellCoord>();
            SpellArea.CollectCells(_grid.Comp, _cellTileStash, spell, position, to, dir, cells);
            
            var range = new List<CellCoord>();
            SpellArea.CollectRangeCells(_grid.Comp, _cellTileStash, spell, position, to, dir, range);
 
            _plannedPathStash.Set(_selection.Entity, new PlannedPathComponent(path));
            _plannedAttackStash.Set(_selection.Entity, new PlannedAttackComponent(to, spell, cells));
            _plannedRangeStash.Set(_selection.Entity, new PlannedRangeComponent(range));
        }
        
        private void BuildMovePath(CellCoord from, CellCoord to, int speed)
        {
            var ready = PathFinder.GetPath(_grid.Comp, from, to, _cellTileStash);
            _plannedPathStash.Set(_selection.Entity, new PlannedPathComponent(PathFinder.GetPathByCost(ready, from, to, speed)));
        }
        
        public void Dispose() {}
    }
}
