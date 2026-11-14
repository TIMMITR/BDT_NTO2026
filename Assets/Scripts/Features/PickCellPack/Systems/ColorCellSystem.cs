using System.Collections.Generic;
using Features.HexGridPack.Components;
using Features.HexGridPack.Data;
using Features.PickCellPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.PickCellPack.Systems
{
    public class ColorCellSystem : ISystem
    {
        public World World { get; set; }

        //private Filter _chosenCellFilter; todo возможно, понадобиться
        private Filter _gridFilter;
        
        //private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<HexGridComponent> _gridStash;
        
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
            /*_chosenCellFilter = World.Filter.
                With<ChosenCellComponent>().
                Build();*/ 
            
            _gridFilter = World.Filter.
                With<HexGridComponent>().
                Build();
            
            //_chosenCellStash = World.GetStash<ChosenCellComponent>();
            _gridStash = World.GetStash<HexGridComponent>();
            
            _colorCellRequest = World.GetRequest<ColorCellRequest>();
            _clearRequest =  World.GetRequest<ClearColorsRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
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
            }

            foreach (var request in _clearRequest.Consume())
            {
                foreach (var cell in _usedCells.Values)
                {
                    _cellPoolParent.GetChild(cell).gameObject.SetActive(false);
                }
            }
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
