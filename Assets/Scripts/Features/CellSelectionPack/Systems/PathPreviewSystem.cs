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
using UnityEngine;

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
            _plannedAttackStash = World.GetStash<PlannedAttackComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            
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

            var unit = _selectedUnitStash.Get(selected).unit;
            
            var from = _positionStash.Get(unit).coord;
            var to = _chosenCellStash.Get(selected).cellCoord; 
            
            _refreshHighlightRequest.Publish(new RefreshHighlightRequest());
            
            if (from == to || GridData.GetUnitTeam(_grid.Comp, _unitStash, to) == TeamSide.Player)
                return;
 
            var speed = _speedStash.Get(unit).currentSpeed;
 
            if (_spellDataStash.Has(unit))
                BuildAttackPath(_spellDataStash.Get(unit).spellData, from, to, speed);
            else
                BuildMovePath(from, to, speed);

            /*
            foreach (var evt in _cellChooseEvent.publishedChanges)
            {
                var gridEntity = _grid.Entity;

                Entity? unit = _selectedUnitStash.Has(gridEntity) ? _selectedUnitStash.Get(gridEntity).unit : null;

                CellCoord? from = unit.HasValue && _positionStash.Has(unit.Value)
                    ? _positionStash.Get(unit.Value).coord : null;
                CellCoord? to = _chosenCellStash.Has(gridEntity) ? _chosenCellStash.Get(gridEntity).cellCoord : null;

                var activeSpell = unit.HasValue && _spellDataStash.Has(unit.Value)
                    ? _spellDataStash.Get(unit.Value).spellData
                    : null;

                if (from == _lastFrom && to == _lastTo && activeSpell == _lastSpell) return;

                _lastFrom = from;
                _lastTo = to;
                _lastSpell = activeSpell;

                _clearColorsRequest.Publish(new ClearColorsRequest()); // полная чистка доски

                if (from.HasValue)
                    _colorCellRequest.Publish(new ColorCellRequest(from.Value, ColorType.Clicked));
                if (to.HasValue)
                    _colorCellRequest.Publish(new ColorCellRequest(to.Value, ColorType.Chosen));

                if (_selectedUnitStash.Has(gridEntity)) // нужно для очистки массива выбранных клеток
                {
                    ref var pickedCellForClear = ref _selectedUnitStash.Get(gridEntity);
                    pickedCellForClear.path.Clear();
                }

                if (from == to || !from.HasValue || !to.HasValue) return;

                ref var selectedUnitComp = ref _selectedUnitStash.Get(gridEntity);
                var speed = selectedUnitComp.moveSpeed;

                if (evt.cellSide == TeamSide.Enemy || evt.cellSide == TeamSide.None)
                {
                    if (activeSpell != null)
                    {
                        BuildAttackPath(activeSpell, from.Value, to.Value, speed, out var path);

                        selectedUnitComp.path = path;
                    }
                    else
                    {
                        BuildPreviewPath(from.Value, to.Value, speed, out var path);

                        selectedUnitComp.path = path;
                    }
                }

            }*/
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
 
            _plannedPathStash.Set(_selection.Entity, new PlannedPathComponent(path));
            _plannedAttackStash.Set(_selection.Entity, new PlannedAttackComponent(to, spell, cells));
        }
        
        private void BuildMovePath(CellCoord from, CellCoord to, int speed)
        {
            var ready = PathFinder.GetPath(_grid.Comp, from, to, _cellTileStash);
            _plannedPathStash.Set(_selection.Entity, new PlannedPathComponent(PathFinder.GetPathByCost(ready, from, to, speed)));
        }
        
        public void Dispose() {}
    }
}
