using System.Collections.Generic;
using Configs;
using Features.CellGridPack.Components;
using Features.CellPack.Components;
using Features.HexGridPack.Data;
using Features.PickCellPack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellGridPack
{
    public sealed class CellGridSystem : IInitializer
    {
        public World World { get; set; }

        private Stash<CellGridComponent> _hexGridStash;
        private Stash<CellTileComponent> _hexTileStash;
        private Stash<ChosenCellComponent> _chosenCellStash;

        private readonly WorldSettingsSo _worldSettings;

        public CellGridSystem(WorldSettingsSo worldSettings)
        {
            _worldSettings = worldSettings;
        }

        public void OnAwake()
        {
            _hexGridStash = World.GetStash<CellGridComponent>();
            _hexTileStash = World.GetStash<CellTileComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();

            CreateGrid();
        }

        private void CreateGrid()
        {
            int gridRadius = _worldSettings.gridRadius;
            float hexRadius = _worldSettings.cellRadius;

            var gridEntity = World.CreateEntity();
            
            _chosenCellStash.Add(gridEntity);
            
            ref var gridComp = ref _hexGridStash.Add(gridEntity);
            gridComp = new CellGridComponent(hexRadius, gridRadius);

            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                int rMin = Mathf.Max(-gridRadius, -q - gridRadius);
                int rMax = Mathf.Min(gridRadius, -q + gridRadius);

                for (int r = rMin; r <= rMax; r++)
                {
                    var tileEntity = World.CreateEntity();
                    _hexTileStash.Add(tileEntity, new CellTileComponent(q, r, _worldSettings.defaultMoveCost, false));
                    
                    gridComp.cells.Add(new CellCoord(q, r), tileEntity);
                }
            }
        }

        public void Dispose() { }
    }
}