using System.Collections.Generic;
using Configs;
using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Requests;
using Features.CellTrackPack.Events;
using Features.FightPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PathFinderPack;
using Features.SpellPack.Components;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Systems
{
    public class PathPreviewSystem : ISystem // todo этот скрипт надо зарефакторить, он выполняет несколько действий одновременно
    {
        public World World { get; set; }

        private Filter _gridFilter;

        private Stash<GridComponent> _gridStash;
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        
        private Stash<CellTileComponent> _cellTileStash;
        private Stash<CellPositionComponent> _positionStash;
        
        private Stash<SpellDataComponent> _spellDataStash;
        
        private Event<CellChooseEvent> _cellChooseEvent;
        
        private Request<ColorCellRequest> _colorCellRequest;
        private Request<ClearColorsRequest> _clearColorsRequest;

        private CellCoord? _lastFrom; 
        private CellCoord? _lastTo;
        private SpellConfigSo _lastSpell;

        private List<CellCoord> _fullPath = new();
        private List<CellCoord> _attackCells = new();

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            
            _cellChooseEvent = World.GetEvent<CellChooseEvent>();
            
            _spellDataStash = World.GetStash<SpellDataComponent>();
            
            _colorCellRequest = World.GetRequest<ColorCellRequest>();
            _clearColorsRequest = World.GetRequest<ClearColorsRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_gridFilter.IsEmpty()) return;

            foreach (var evt in _cellChooseEvent.publishedChanges)
            {
                var gridEntity = _gridFilter.First();

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

                ref var gridComp = ref _gridStash.Get(gridEntity);
                ref var selectedUnitComp = ref _selectedUnitStash.Get(gridEntity);
                var speed = selectedUnitComp.moveSpeed;
                
                if (evt.cellSide == TeamSide.Enemy || evt.cellSide == TeamSide.None)
                {
                    if (activeSpell != null)
                    {
                        BuildAttackPath(gridComp, activeSpell, from.Value, to.Value, speed, out var path);
                    
                        selectedUnitComp.path = path;
                    }
                    else
                    {   
                        BuildPreviewPath(ref gridComp, from.Value, to.Value, speed, out var path);
                        
                        selectedUnitComp.path = path;
                    }
                }
                
            }
        }

        private void BuildAttackPath(GridComponent gridComp, SpellConfigSo spell, CellCoord from, CellCoord to, int speed,
            out List<CellCoord> resultPath)
        {
            var foundPath = PathFinder.GetPath(gridComp, from, to, _cellTileStash);
            resultPath = new();

            CellCoord best = default;
            CellCoord bestDir = default;
            var bestCost = int.MaxValue;
 
            foreach (var pair in foundPath.cellsCost)
            {
                if (pair.Value > speed || pair.Value >= bestCost)
                    continue;
                if (!SpellArea.TryAim(gridComp, _cellTileStash, spell, pair.Key, to, out var dir))
                    continue;
 
                best = pair.Key;
                bestDir = dir;
                bestCost = pair.Value;
            }
 
            if (bestCost == int.MaxValue) 
                return;

            if (best != from)
                resultPath = PathFinder.ReconstructPath(foundPath, from, to);
            
            foreach (var cell in resultPath)
                _colorCellRequest.Publish(new ColorCellRequest(cell, ColorType.Path));

            _attackCells.Clear();
            SpellArea.CollectCells(gridComp, _cellTileStash, spell, from, to, bestDir, _attackCells);
            
            foreach (var cell in _attackCells)
                _colorCellRequest.Publish(new ColorCellRequest(cell, ColorType.Attack)); 
        }
        

        private void BuildPreviewPath(ref GridComponent gridComp, CellCoord from, CellCoord to, int maxCost, out List<CellCoord> path)
        {
            var pathReady = PathFinder.GetPath(gridComp, from, to, _cellTileStash);
            
            _fullPath = PathFinder.ReconstructPath(pathReady, from, to);

            path = new()
            {
                from
            };

            foreach (var cell in _fullPath)
            {
                if(pathReady.cellsCost[cell] > maxCost) break;
                
                path.Add(cell);
                _colorCellRequest.Publish(new ColorCellRequest(cell, ColorType.Path));
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
