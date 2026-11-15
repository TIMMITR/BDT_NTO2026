using System.Collections.Generic;
using Features.CellGridPack.Components;
using Features.CellPack.Components;
using Features.HexGridPack.Data;
using Features.PathFinderPack;
using Features.PickCellPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellPack.Systems
{
    public class ColorCellSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        
        private Stash<PickedCellComponent> _pickedCellStash;
        private Stash<CellGridComponent> _gridStash;
        
        private Request<ColorCellRequest> _colorCellRequest;
        private Request<ClearColorsRequest> _clearRequest;
        
        private readonly Transform _cellPoolParent;
        private readonly int _poolSize;
        
        private Dictionary<CellCoord, int> _usedCells = new();

        private int _currentCellIndex;
        
        public ColorCellSystem(Transform cellPoolParent)
        {
            _cellPoolParent = cellPoolParent;
            _poolSize = _cellPoolParent.childCount;
        }

        public void OnAwake()
        {
            _gridFilter = World.Filter.
                With<CellGridComponent>().
                Build();
            
            _gridStash = World.GetStash<CellGridComponent>();
            _pickedCellStash = World.GetStash<PickedCellComponent>();
            
            _colorCellRequest = World.GetRequest<ColorCellRequest>();
            _clearRequest =  World.GetRequest<ClearColorsRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _clearRequest.Consume())
            {
                foreach (var cell in _usedCells.Values)
                {
                    _cellPoolParent.GetChild(cell).gameObject.SetActive(false);
                }
                
                _usedCells.Clear();
                _currentCellIndex = 0;
            }
            
            foreach (var request in _colorCellRequest.Consume())
            {
                Entity gridEntity = _gridFilter.First(); // todo возможно, стоит вынести в onAwake
                ref var gridComp = ref _gridStash.Get(gridEntity);

                Vector3 targetCoordinates = gridComp.ToWorldCoord(request.cellCoord);
                int poolCellIndex = 0;

                if (_usedCells.TryGetValue(request.cellCoord, out var usedCellIndex))
                {
                    poolCellIndex = usedCellIndex;
                }
                else
                {
                    poolCellIndex = ChooseNextCellIndex();
                    _usedCells.Add(request.cellCoord, poolCellIndex);
                }
                
                var child = _cellPoolParent.GetChild(poolCellIndex);
                
                child.gameObject.SetActive(true);
                child.position = new Vector3(targetCoordinates.x, child.position.y, targetCoordinates.z);

                if (_pickedCellStash.Has(gridEntity))
                {
                    //PathFinder.GetPath()
                }
            }
        }

        private void DrawLine(CellCoord from, CellCoord to)
        {
            
        }

        private int ChooseNextCellIndex()
        {
            return _currentCellIndex = _currentCellIndex + 2 > _poolSize ?  0 : _currentCellIndex + 1;
        }
    
        public void Dispose()
        {
            
        }
    }
}
