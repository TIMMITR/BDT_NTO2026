using System.Collections.Generic;
using Configs;
using Features.HexGridPack.Components;
using Features.HexGridPack.Data;
using Features.PickCellPack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.HexGridPack
{
    public sealed class HexGridSystem : IInitializer
    {
        public World World { get; set; }

        private Stash<HexGridComponent> _hexGridStash;
        private Stash<HexTileComponent> _hexTileStash;
        private Stash<ChosenCellComponent> _chosenCellStash;

        private readonly GameObject _boardGameObject;
        private readonly WorldSettingsSo _worldSettings;

        public HexGridSystem(GameObject boardGameObject, WorldSettingsSo worldSettings)
        {
            _boardGameObject = boardGameObject;
            _worldSettings = worldSettings;
        }

        public void OnAwake()
        {
            _hexGridStash = World.GetStash<HexGridComponent>();
            _hexTileStash = World.GetStash<HexTileComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();

            CreateGrid();
        }

        private void CreateGrid()
        {
            
            if (_boardGameObject == null)
            {
                Debug.LogWarning("[HEX_GRID_SYSTEM] No GridStartPointView found on " + _boardGameObject.name);
                return;
            }
            
            int gridRadius = _worldSettings.gridRadius;
            float hexRadius = _worldSettings.cellRadius;

            var gridEntity = World.CreateEntity();
            
            _chosenCellStash.Add(gridEntity);
            ref var gridComp = ref _hexGridStash.Add(gridEntity);
            gridComp.root = _boardGameObject.transform;
            gridComp.gridRadius = gridRadius;
            gridComp.hexRadius = hexRadius;
            gridComp.cells = new Dictionary<CellCoord, Entity>();
            gridComp.takenCells = new Dictionary<CellCoord, Entity>();

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
                    tileComp.moveCost = _worldSettings.defaultMoveCost;
                    
                    gridComp.cells.Add(new CellCoord(q, r), tileEntity);
                }
            }
        }

        public void Dispose() { }
    }
}