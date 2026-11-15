using System.Collections.Generic;
using Features.CellGridPack.Components;
using Features.CellPack.Components;
using Features.HexGridPack.Data;
using Features.PathFinderPack;
using Features.PickCellPack;
using Features.PickCellPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellPack.Systems
{
    public class PathPreviewSystem : ISystem
    {
        public World World { get; set; }

        private Filter _cellGridFilter;

        private Stash<CellGridComponent> _cellGridStash;
        private Stash<PickedCellComponent> _pickedCellStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        
        private Stash<CellTileComponent> _cellTileStash; //todo стоит поменять на Dict gridCell
        
        private Request<ColorCellRequest> _colorCellRequest;
        private Request<ClearColorsRequest> _clearColorsRequest;

        private CellCoord? _lastFrom; 
        private CellCoord? _lastTo;
        
        private readonly Transform _cellPoolParent;

        private List<CellCoord> _path = new();
        
        private readonly int _poolSize;
        private int _currentCellIndex;
        
        public PathPreviewSystem(Transform cellPoolParent)
        {
            _cellPoolParent = cellPoolParent;
            _poolSize = _cellPoolParent.childCount;
        }

        public void OnAwake()
        {
            _cellGridFilter = World.Filter.With<CellGridComponent>().Build();
            
            _cellGridStash = World.GetStash<CellGridComponent>();
            _pickedCellStash = World.GetStash<PickedCellComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            
            _colorCellRequest = World.GetRequest<ColorCellRequest>();
            _clearColorsRequest = World.GetRequest<ClearColorsRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_cellGridFilter.IsEmpty()) return;

            var gridEntity = _cellGridFilter.First();

            CellCoord? from = _pickedCellStash.Has(gridEntity) ? _pickedCellStash.Get(gridEntity).cellCoord : null;
            CellCoord? to = _chosenCellStash.Has(gridEntity) ? _chosenCellStash.Get(gridEntity).cellCoord : null;

            if (from == _lastFrom && to == _lastTo) return;

            _lastFrom = from;
            _lastTo = to;

            _clearColorsRequest.Publish(new ClearColorsRequest()); // полная чистка доски

            if (from.HasValue)
                _colorCellRequest.Publish(new ColorCellRequest {cellCoord = from.Value});
            if (to.HasValue)
                _colorCellRequest.Publish(new ColorCellRequest {cellCoord = to.Value});

            if (from == to || !from.HasValue || !to.HasValue) return;

            SendDrawPassRequest(gridEntity, from.Value, to.Value);
        }

        private void SendDrawPassRequest(Entity entity, CellCoord from, CellCoord to)
        {
            ref var gridComp = ref _cellGridStash.Get(entity);
            var pathReady = PathFinder.GetPath(gridComp, from, to, _cellTileStash);
            
            _path = PathFinder.ReconstructPath(pathReady, from, to);

            foreach (var cell in _path)
            {
                _colorCellRequest.Publish(new ColorCellRequest {cellCoord = cell});
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
