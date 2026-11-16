using System.Collections.Generic;
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
        private Stash<PickedCellComponent> _pickedCellStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        
        private Stash<CellTileComponent> _cellTileStash; //todo стоит поменять на Dict gridCell
        
        private Request<ColorCellRequest> _colorCellRequest;
        private Request<ClearColorsRequest> _clearColorsRequest;

        private CellCoord? _lastFrom; 
        private CellCoord? _lastTo;

        private List<CellCoord> _fullPath = new();

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _pickedCellStash = World.GetStash<PickedCellComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            
            _colorCellRequest = World.GetRequest<ColorCellRequest>();
            _clearColorsRequest = World.GetRequest<ClearColorsRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_gridFilter.IsEmpty()) return;

            var gridEntity = _gridFilter.First();

            CellCoord? from = _pickedCellStash.Has(gridEntity) ? _pickedCellStash.Get(gridEntity).cellCoord : null;
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

            if (from == to || !from.HasValue || !to.HasValue) return;

            ref var pickedCellComp = ref _pickedCellStash.Get(gridEntity);
            var speed = pickedCellComp.moveSpeed;
            
            BuildPreviewPath(gridEntity, from.Value, to.Value, speed, out var points);
            
            pickedCellComp.points = points; // todo возможно, стоит расчет пути вынести в отдельную систему
        }

        private void BuildPreviewPath(Entity entity, CellCoord from, CellCoord to, int maxCost, out List<CellCoord> points)
        {
            ref var gridComp = ref _gridStash.Get(entity);
            var pathReady = PathFinder.GetPath(gridComp, from, to, _cellTileStash);
            
            _fullPath = PathFinder.ReconstructPath(pathReady, from, to);

            points = new()
            {
                from
            };

            foreach (var cell in _fullPath)
            {
                if(pathReady.cellsCost[cell] > maxCost) break;
                
                points.Add(cell);
                _colorCellRequest.Publish(new ColorCellRequest {cellCoord = cell});
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
