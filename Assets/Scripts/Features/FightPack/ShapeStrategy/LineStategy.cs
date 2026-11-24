using System.Collections.Generic;
using Configs;
using Features.CalcPack;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.FightPack.ShapeStrategy
{
    public class LineStrategy : IShapeStrategy
    {
        public bool TryAim(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to, out CellCoord dir)
        {
            foreach (var d in HexMath.Directions)
            {
                foreach (var cell in GetPath(gridComp, tiles, spell, from, d))
                {
                    if (cell != to)
                        continue;

                    dir = d;
                    
                    return true;
                }
            }
            
            dir = default;
            return false;
        }

        public void CollectCells(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell,
            CellCoord from, CellCoord to, CellCoord dir, List<CellCoord> result)
        {
            result.AddRange(GetPath(gridComp, tiles, spell, from, dir));
        }

        public CellCoord GetStartPoint(CellCoord from, CellCoord to) => from;

        private IEnumerable<CellCoord> GetPath(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell,
            CellCoord from, CellCoord dir)
        {
            var start = from;
            
            for (int i = 0; i < spell.range; i++)
            {
                start += dir;
                
                if (!GridData.Contains(gridComp, start))
                    yield break;
                
                yield return start;
                
                if (GridData.HasBuilding(gridComp, tiles, start) || (!spell.isMultipleHit &&  GridData.HasUnit(gridComp, start)))
                    yield break; // после, чтобы зарегало урон по юниту
            }
        }
    }
}
