using System.Collections.Generic;
using Features.HexGridPack.Components;
using Features.HexGridPack.Data;
using Features.HexGridPack.Views;
using Features.LevelPack.Components;
using Features.LevelPack.SO;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.HexGridPack
{
    public sealed class HexGridSystem : IInitializer
    {
        public World World { get; set; }

        private Stash<HexGridComponent> _hexGridStash;
        private Stash<HexTileComponent> _hexTileStash;
        private Stash<BlockedTag> _blockedCellStash;
        private Stash<ChosenCellComponent> _chosenCellStash;

        private readonly Transform _gridStartPoint;
        private readonly GameObject _boardGameObject;
        private readonly LevelConfig _levelConfig;

        public HexGridSystem(GameObject boardGameObject, LevelConfig levelConfig)
        {
            _boardGameObject = boardGameObject;
            _levelConfig = levelConfig;
        }

        public void OnAwake()
        {
            _hexGridStash = World.GetStash<HexGridComponent>();
            _hexTileStash = World.GetStash<HexTileComponent>();
            _blockedCellStash = World.GetStash<BlockedTag>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();

            CreateGrid();
        }

        private void CreateGrid()
        {
            if (!_levelConfig)
            {
                Debug.Log("[HEX_GRID_SYSTEM] No level config found.");
                return;
            }
            
            var startPoint = _boardGameObject.GetComponentInChildren<GridStartPointView>();
            
            if (!startPoint)
            {
                Debug.LogWarning("[HEX_GRID_SYSTEM] No GridStartPointView found on " + _boardGameObject.name);
                return;
            }
            
            int gridRadius = _levelConfig.GridRadius;
            float hexRadius = _levelConfig.HexRadius;

            var gridEntity = World.CreateEntity();
            
            _chosenCellStash.Add(gridEntity);
            ref var gridComp = ref _hexGridStash.Add(gridEntity);
            gridComp.root = startPoint.transform;
            gridComp.gridRadius = gridRadius;
            gridComp.hexRadius = hexRadius;
            gridComp.cells = new Dictionary<CellCoord, Entity>();
            gridComp.takenCells = new Dictionary<CellCoord, Entity>(); // todo возможно стоит вынести в скрипт с генерацией персонажей

            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                int rMin = Mathf.Max(-gridRadius, -q - gridRadius);
                int rMax = Mathf.Min(gridRadius, -q + gridRadius);

                for (int r = rMin; r <= rMax; r++)
                {
                    var tileEntity = World.CreateEntity();
                    ref var tileComp = ref _hexTileStash.Add(tileEntity);
                    tileComp.q = q;
                    tileComp.r = r;
                    tileComp.moveCost = _levelConfig.DefaultMoveCost;
                    
                    gridComp.cells.Add(new CellCoord(q, r), tileEntity);
                }
            }
            
            ActivateCustomCells(gridComp.cells);
        }

        private void ActivateCustomCells(Dictionary<CellCoord, Entity> tiles)
        {
            foreach (var cell in _levelConfig.CustomCells)
            {
                var coord = new CellCoord(cell.q, cell.r);
    
                if (!tiles.TryGetValue(coord, out var tileEntity))
                {
                    Debug.Log($"[HEX_GRID_SYSTEM] Cell:{cell.q}, {cell.r} is not initted.", _levelConfig);
                    continue;
                }

                if (cell.blocked)
                {
                    _blockedCellStash.Add(tileEntity); // todo Возможно стот хранить данные о блоке в компоненте клетки
                }
                else if (cell.moveCost > 1)
                {
                    ref var tileComp = ref _hexTileStash.Get(tileEntity);
                    tileComp.moveCost = cell.moveCost;
                }
            }
        }

        public void Dispose() { }
    }
}