using Features.CellGridPack.Components;
using Features.CellPack.Components;
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
        
        private readonly Transform _cellPoolParent;
        
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
        }

        public void OnUpdate(float deltaTime)
        {
            if (_cellGridFilter.IsEmpty()) return;
            
            var gridEntity = _cellGridFilter.First();
            
            if (!_pickedCellStash.Has(gridEntity) || !_chosenCellStash.Has(gridEntity)) return;
            
            ref var gridComp = ref _cellGridStash.Get(gridEntity);
            ref var pickedCellComp = ref _pickedCellStash.Get(gridEntity);
            ref var chosenCellComp = ref _chosenCellStash.Get(gridEntity);

            var from = pickedCellComp.cellCoord;
            var to = chosenCellComp.cellCoord;

            _colorCellRequest.Publish(new ColorCellRequest {cellCoord = from});
            
            if (from == to) return;
            
            var pathReady = PathFinder.GetPath(gridComp, from, to, _cellTileStash);
            
            var reconstructedPath = PathFinder.ReconstructPath(pathReady, from, to);

            foreach (var cell in reconstructedPath)
            {
                _colorCellRequest.Publish(new ColorCellRequest {cellCoord = cell});
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
