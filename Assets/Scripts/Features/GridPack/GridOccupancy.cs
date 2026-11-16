using Features.GridPack.Components;
using Features.GridPack.Data;
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
            CellCoord at)
        {
            grid.takenCells[at] = unit;
            tiles.Get(grid.cells[at]).blocked = true;
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
            tiles.Get(grid.cells[from]).blocked = false;
 
            grid.takenCells[to] = unit;
            tiles.Get(grid.cells[to]).blocked = true;
 
            ref var pos = ref positions.Get(unit);
            pos.coord = to;
        }
    }
}
