using System.Collections.Generic;
using Configs;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.FightPack.ShapeStrategy
{
    public class ConeStrategy : IShapeStrategy
    {
        public bool TryAim(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to, out CellCoord dir)
        {
            dir = from.DirectionTo(to);
            
            return from.DistanceTo(to) <= spell.range;
        }

        public void CollectCells(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to, CellCoord dir,
            List<CellCoord> result)
        {
            for (int i = 0; i < spell.range; i++)
            {
                // todo доделать
            }
        }

        public void CollectRangeCells(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to, CellCoord dir, List<CellCoord> result) { }

        public CellCoord GetStartPoint(CellCoord from, CellCoord to) => from;
    }
}
