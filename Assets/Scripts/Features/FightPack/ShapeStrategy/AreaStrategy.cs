using System;
using System.Collections.Generic;
using Configs;
using Features.CalcPack;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.FightPack.ShapeStrategy
{
    public class AreaStrategy : IShapeStrategy
    {
        public bool TryAim(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to, out CellCoord dir)
        {
            dir = default;
            
            if (!GridData.Contains(gridComp, from))
                return false;
            
            dir = from.DirectionTo(to);

            return from.DistanceTo(to) <= spell.range + spell.areaRadius + 1;
        }

        public void CollectCells(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to,
            CellCoord dir, List<CellCoord> result)
        {
            foreach (var cell in HexMath.GetRange(to, spell.areaRadius ))
                if (GridData.Contains(gridComp, cell))
                    result.Add(cell);
        }

        public void CollectRangeCells(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell,
            CellCoord from, CellCoord to, CellCoord dir, List<CellCoord> result)
        {
            var cell = from;
 
            for (var i = 0; i < spell.range; i++)
            {
                cell += cell.DirectionTo(to);
 
                if (!GridData.Contains(gridComp, cell) || cell.DistanceTo(to) <= spell.areaRadius)
                    break;
 
                result.Add(cell);
            }
        }

        public CellCoord GetStartPoint(CellCoord from, CellCoord to) => to;
    }
}
