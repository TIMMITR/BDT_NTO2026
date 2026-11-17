using System.Collections.Generic;
using Configs;
using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Requests;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PathFinderPack;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Systems
{
    public class PathPreviewSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;

        private Stash<GridComponent> _gridStash;
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        
        private Stash<CellTileComponent> _cellTileStash;
        private Stash<CellPositionComponent> _positionStash;
        
        private Request<ColorCellRequest> _colorCellRequest;
        private Request<ClearColorsRequest> _clearColorsRequest;

        private CellCoord? _lastFrom; 
        private CellCoord? _lastTo;

        private List<CellCoord> _fullPath = new();

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            
            _colorCellRequest = World.GetRequest<ColorCellRequest>();
            _clearColorsRequest = World.GetRequest<ClearColorsRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_gridFilter.IsEmpty()) return;

            var gridEntity = _gridFilter.First();

            Entity? unit = _selectedUnitStash.Has(gridEntity) ? _selectedUnitStash.Get(gridEntity).unit : null;

            CellCoord? from = unit.HasValue && _positionStash.Has(unit.Value)
                ? _positionStash.Get(unit.Value).coord : null;
            CellCoord? to = _chosenCellStash.Has(gridEntity) ? _chosenCellStash.Get(gridEntity).cellCoord : null;

            if (from == _lastFrom && to == _lastTo) return;
            
            _lastFrom = from;
            _lastTo = to;

            _clearColorsRequest.Publish(new ClearColorsRequest()); // полная чистка доски

            if (from.HasValue)
                _colorCellRequest.Publish(new ColorCellRequest {cellCoord = from.Value});
            if (to.HasValue && !from.HasValue) // проверка на !from нужна,
                                               // чтобы не отрисовывать выбранную клетку, при построении пути
                _colorCellRequest.Publish(new ColorCellRequest {cellCoord = to.Value});

            if (_selectedUnitStash.Has(gridEntity)) // нужно для очистки массива выбранных клеток
            {
                ref var pickedCellForClear = ref _selectedUnitStash.Get(gridEntity);
                pickedCellForClear.path.Clear();
            }

            if (from == to || !from.HasValue || !to.HasValue) return;

            ref var selectedUnitComp = ref _selectedUnitStash.Get(gridEntity);
            var speed = selectedUnitComp.moveSpeed;

            if (selectedUnitComp.teamSide == TeamSide.Player)
            {
                BuildPreviewPath(gridEntity, from.Value, to.Value, speed, out var path);
            
                selectedUnitComp.path = path; // todo возможно, стоит расчет пути вынести в отдельную систему
            }
            else
            {
                
            }
        }

        private void BuildPreviewPath(Entity entity, CellCoord from, CellCoord to, int maxCost, out List<CellCoord> path)
        {
            ref var gridComp = ref _gridStash.Get(entity);
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
                _colorCellRequest.Publish(new ColorCellRequest {cellCoord = cell});
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
