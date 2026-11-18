using System.Collections.Generic;
using Configs;
using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Requests;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.MeshPack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellSelectionPack.Systems
{
    public class ColorCellSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        
        private Stash<SelectedUnitComponent> _pickedCellStash;
        private Stash<GridComponent> _gridStash;
        private Stash<MeshRendererComponent> _meshRendererStash;
        
        private Request<ColorCellRequest> _colorCellRequest;
        private Request<ClearColorsRequest> _clearRequest;
        
        private readonly Transform _cellPoolParent;
        private readonly int _poolSize;
        private readonly ColorData _colorData;
        
        private Dictionary<CellCoord, int> _usedCells = new();

        private int _currentCellIndex;
        
        public ColorCellSystem(Transform cellPoolParent, ColorData colorData)
        {
            _cellPoolParent = cellPoolParent;
            _poolSize = _cellPoolParent.childCount;
            _colorData = colorData;
        }

        public void OnAwake()
        {
            _gridFilter = World.Filter.
                With<GridComponent>().
                Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _pickedCellStash = World.GetStash<SelectedUnitComponent>();
            _meshRendererStash = World.GetStash<MeshRendererComponent>();
            
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

                
                Color color = _colorData.chosenColor;

                switch (request.colorType)
                {
                    case ColorType.Chosen:
                        color = _colorData.chosenColor;
                        break;
                    case ColorType.Clicked:
                        color = _colorData.clickedColor;
                        break;
                    case ColorType.Path:
                        color = _colorData.pathColor;
                        break;
                    case ColorType.Attack:
                        color = _colorData.attackColor;
                        break;
                }

                child.GetComponent<MeshRenderer>().material.color = color;

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
