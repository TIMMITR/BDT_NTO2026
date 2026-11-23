using Configs;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.GridPack
{
    public static class GridData
    {
        public static bool Contains(GridComponent grid, CellCoord cell) => grid.cells.ContainsKey(cell);
        public static bool HasUnit (GridComponent grid, CellCoord cell) => grid.takenCells.ContainsKey(cell);
        public static bool HasBuilding(GridComponent grid, Stash<CellTileComponent> tiles, CellCoord cell) =>
            !grid.cells.TryGetValue(cell, out var tile) || tiles.Get(tile).blocked;
        
        public static bool IsFreeCell(GridComponent grid, Stash<CellTileComponent> tiles, CellCoord cell) =>
            !HasUnit(grid, cell) && !HasBuilding(grid, tiles, cell);
        
        public static bool TryGetUnit(GridComponent grid, Stash<UnitComponent> units, CellCoord cell, out Entity unit) =>
            grid.takenCells.TryGetValue(cell, out unit) && units.Has(unit); // защита от препятствий
        
        public static bool TryGetUnit(GridComponent grid, Stash<UnitComponent> units, CellCoord cell,
            out Entity unit, out TeamSide side)
        {
            if (TryGetUnit(grid, units, cell, out unit))
            {
                side = units.Get(unit).teamSide;
                return true;
            }
 
            side = TeamSide.None;
            return false;
        }
        
        public static TeamSide GetUnitTeam(GridComponent grid, Stash<UnitComponent> units, CellCoord cell) => TryGetUnit(grid, units, cell, out var unit)
            ? units.Get(unit).teamSide
            : TeamSide.None;
    }
}