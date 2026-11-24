using Configs;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GridPack.Systems
{
    public class GridSystem : IInitializer // todo надо ли делать sealed?
    {
        public World World { get; set; }

        private Stash<GridComponent> _gridStash;
        private Stash<CellTileComponent> _cellTileStash;

        private readonly WorldSettingsSo _worldSettings;

        public GridSystem(WorldSettingsSo worldSettings)
        {
            _worldSettings = worldSettings;
        }

        public void OnAwake()
        {
            _gridStash = World.GetStash<GridComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();

            CreateGrid();
        }

        private void CreateGrid()
        {
            int gridRadius = _worldSettings.gridRadius;
            float cellRadius = _worldSettings.cellRadius;

            var gridEntity = World.CreateEntity();
            
            ref var gridComp = ref _gridStash.Add(gridEntity);
            gridComp = new GridComponent(cellRadius, gridRadius);

            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                int rMin = Mathf.Max(-gridRadius, -q - gridRadius);
                int rMax = Mathf.Min(gridRadius, -q + gridRadius);

                for (int r = rMin; r <= rMax; r++)
                {
                    var tileEntity = World.CreateEntity();
                    _cellTileStash.Add(tileEntity, new CellTileComponent(q, r, _worldSettings.defaultMoveCost, false));
                    
                    gridComp.cells.Add(new CellCoord(q, r), tileEntity);
                }
            }
        }

        public void Dispose() { }
    }
}