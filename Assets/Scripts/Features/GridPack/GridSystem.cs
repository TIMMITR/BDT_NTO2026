using Configs;
using Features.CellSelectionPack.Components;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GridPack
{
    public sealed class CellGridSystem : IInitializer
    {
        public World World { get; set; }

        private Stash<GridComponent> _hexGridStash;
        private Stash<CellTileComponent> _hexTileStash;
        private Stash<HoveredCellComponent> _chosenCellStash;

        private readonly WorldSettingsSo _worldSettings;

        public CellGridSystem(WorldSettingsSo worldSettings)
        {
            _worldSettings = worldSettings;
        }

        public void OnAwake()
        {
            _hexGridStash = World.GetStash<GridComponent>();
            _hexTileStash = World.GetStash<CellTileComponent>();
            _chosenCellStash = World.GetStash<HoveredCellComponent>();

            CreateGrid();
        }

        private void CreateGrid()
        {
            int gridRadius = _worldSettings.gridRadius;
            float hexRadius = _worldSettings.cellRadius;

            var gridEntity = World.CreateEntity();
            
            _chosenCellStash.Add(gridEntity);
            
            ref var gridComp = ref _hexGridStash.Add(gridEntity);
            gridComp = new GridComponent(hexRadius, gridRadius);

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