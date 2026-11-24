using System.Collections.Generic;
using Configs;
using Features.FightPack.ShapeStrategy;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.FightPack
{
    public static class SpellArea // todo возможно, стоит объеденить переменные (from, to) (gridComp, spell)
    {
        private static List<CellCoord> _cells = new ();
        
        private static readonly Dictionary<SpellRangeType, IShapeStrategy> Strategies = new()
        {
            { SpellRangeType.Straight, new LineStrategy() },
            { SpellRangeType.Area, new AreaStrategy() },
        };

        public static bool TryAim(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to,
            out CellCoord dir)
        {
            dir = default;
            return Strategies.TryGetValue(spell.rangeType, out var shape) && shape.TryAim(gridComp, tiles, spell, from, to, out dir);
        }

        public static void CollectCells(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell,
            CellCoord from, CellCoord to, CellCoord dir, List<CellCoord> result)
        {
            if (Strategies.TryGetValue(spell.rangeType, out var shape))
                shape.CollectCells(gridComp, tiles, spell, from, to, dir, result);
        }

        public static void CollectTargets(GridComponent gridComp, Stash<UnitComponent> units, Stash<CellTileComponent> tiles, SpellConfigSo spell,
            CellCoord from, CellCoord to,
            CellCoord dir, TeamSide team, List<Entity> result)
        {
            _cells.Clear();
            
            CollectCells(gridComp, tiles, spell, from, to, dir, _cells);

            foreach (var cell in _cells)
            {
                if (!GridData.TryGetUnit(gridComp, units, cell, out var unit))
                    continue;
                if (!spell.canHitAllies && units.Get(unit).teamSide == team)
                    continue;
                
                result.Add(unit);
            }
        }

        public static CellCoord GetStartPoint(SpellConfigSo spell, CellCoord from, CellCoord to) => 
            Strategies.TryGetValue(spell.rangeType, out var shape) 
            ? shape.GetStartPoint(from, to) 
            : from;
    }
}
