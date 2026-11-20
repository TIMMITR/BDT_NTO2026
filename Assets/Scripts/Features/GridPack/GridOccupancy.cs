using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.GridPack
{
    public static class GridOccupancy
    {
        public static void Place(
            ref GridComponent grid,
            Stash<CellTileComponent> tiles,
            Stash<CellPositionComponent> positions,
            Entity unit,
            CellCoord at,
            bool isBarricade) // todo перименовать
        {
            grid.takenCells[at] = unit;
            tiles.Get(grid.cells[at]).blocked = isBarricade;
            positions.Set(unit, new CellPositionComponent { coord = at });
        }
 
        public static void Move(
            ref GridComponent grid,
            Stash<CellTileComponent> tiles,
            Stash<CellPositionComponent> positions,
            Entity unit,
            CellCoord from,
            CellCoord to)
        {
            grid.takenCells.Remove(from);
            //tiles.Get(grid.cells[from]).blocked = false;
 
            grid.takenCells[to] = unit;
            //tiles.Get(grid.cells[to]).blocked = true;
 
            ref var pos = ref positions.Get(unit);
            pos.coord = to;
        }
        
        public static bool TryGetUnit(
            GridComponent grid,
            Stash<UnitComponent> units,
            CellCoord at,
            out Entity unit)
        {
            return grid.takenCells.TryGetValue(at, out unit) && units.Has(unit);
        }
    }
}
