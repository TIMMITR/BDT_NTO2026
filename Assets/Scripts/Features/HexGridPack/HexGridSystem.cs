using System.Collections.Generic;
using Features.HexGridPack.Components;
using Features.HexGridPack.Views;
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
            ref var gridComp = ref _hexGridStash.Add(gridEntity);
            gridComp.root = startPoint.transform;
            gridComp.gridRadius = gridRadius;
            gridComp.hexRadius = hexRadius;
            
            var tiles = new Dictionary<(int q, int r), Entity>();

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
                    
                    tiles.Add((q, r), tileEntity);
                }
            }
            
            ActivateCustomCells(tiles);
        }

        private void ActivateCustomCells(Dictionary<(int q, int r), Entity> tiles)
        {
            var applied = new HashSet<(int q, int r)>();
 
            foreach (var cell in _levelConfig.CustomCells)
            {
                var key = (cell.q, cell.r);
 
                if (!tiles.TryGetValue(key, out var tileEntity))
                {
                    Debug.Log($"[HEX_GRID_SYSTEM] Cell ({cell.q}, {cell.r}) is outside the grid", _levelConfig);
                    continue;
                }
 
                if (!applied.Add(key))
                {
                    Debug.LogWarning($"[HEX_GRID_SYSTEM] Cell ({cell.q}, {cell.r}) is listed more than once", _levelConfig);
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

        public void Dispose()
        {
            _hexGridStash = null;
            _hexTileStash = null;
        }
    }
}