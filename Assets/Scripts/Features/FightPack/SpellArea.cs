using System;
using System.Collections.Generic;
using Configs;
using Features.CalcPack;
using Features.CellSelectionPack.Components;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PathFinderPack;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.FightPack
{
    public static class SpellArea
    {
        public static bool TryHitFrom(GridComponent grid, Stash<CellTileComponent> tiles,
            SpellConfigSo spell, CellCoord from, CellCoord to,
            out CellCoord direction)
        {
            direction = default;

            switch (spell.rangeType)
            {
                case SpellRangeType.Straight:
                    return TryLineTarget(grid, tiles, from, to, spell, out direction);
                case SpellRangeType.Area:
                    if (from.DistanceTo(to) <= spell.range)
                    {
                        direction = GetDirection(from, to);
                        return true;
                    }
                    return false;
                case SpellRangeType.Cone: // todo сделать
                    break;
            }
            
            return false;
        }

        public static List<Entity> CollectTargets(GridComponent grid, Stash<CellTileComponent> tiles, Stash<UnitComponent> units,
            AttackDataComponent attackData,
            TeamSide casterSide)
        {
            List<Entity> targets = new();

            var spell = attackData.spellConfig;
            var from = attackData.from;
            var to = attackData.to;
            var direction = attackData.dir;

            switch (spell.rangeType)
            {
                case SpellRangeType.Straight:
                    CollectLineTarget(grid, tiles, units, spell, from, to, direction, targets, casterSide);
                    break;
                case SpellRangeType.Area:
                    CollectAreaTarget(grid, tiles, units, spell, to, targets, casterSide);
                    break;
                case SpellRangeType.Cone:
                    break;
            }

            return targets;
        }

        public static List<CellCoord> ReconstructPath(GridComponent grid, Stash<CellTileComponent> tiles,
            SpellConfigSo spell,
            CellCoord from, CellCoord to, CellCoord dir) // todo возможно to не нужен
        {
            List<CellCoord> cells = new List<CellCoord>();

            switch (spell.rangeType)
            {
                case SpellRangeType.Straight:
                    ReconstructLine(grid, tiles, spell, from, dir, cells);
                    break;
                case  SpellRangeType.Area:
                    ReconstructArea(grid, spell, to, cells);
                    break;
                case SpellRangeType.Cone:
                    break;
            }

            return cells;
        }
        
        private static void ReconstructLine(GridComponent gridComp, Stash<CellTileComponent> tiles,
            SpellConfigSo spell,
            CellCoord from, CellCoord direction, List<CellCoord> cells)
        {
            var cell = from;
 
            for (var i = 0; i < spell.range; i++)
            {
                cell = new CellCoord(cell.Q + direction.Q, cell.R + direction.R);
 
                if (!gridComp.cells.TryGetValue(cell, out var cellEntity))
                    break;
 
                cells.Add(cell);            
 
                if (!spell.isMultipleHit && GridData.HasUnit(gridComp, cell))
                    break;
            }
        }
        
        private static void ReconstructArea(GridComponent gridComp, 
            SpellConfigSo spell,
            CellCoord center, List<CellCoord> cells)
        {
            var radius = spell.areaRadius;
            
            for (var dq = -radius; dq <= radius; dq++)
            {
                var rMin = Math.Max(-radius, -dq - radius);
                var rMax = Math.Min(radius, -dq + radius);
 
                for (var dr = rMin; dr <= rMax; dr++)
                {
                    var cell = new CellCoord(center.Q + dq, center.R + dr);
                    
                    if (gridComp.cells.ContainsKey(cell))
                        cells.Add(cell);
                }
            }
        }

        private static void CollectLineTarget(GridComponent gridComp, Stash<CellTileComponent> tiles, Stash<UnitComponent> units,
            SpellConfigSo spell,
            CellCoord from, CellCoord to, CellCoord direction,
            List<Entity> targets,
            TeamSide casterSide) //todo перепроверить
        {
            var cell = from;
            
            for (int i = 0; i < spell.range; i++)
            {
                cell = new CellCoord(cell.Q + direction.Q, cell.R + direction.R);

                if (!GridData.IsFreeCell(gridComp, tiles, cell))
                    return;
                
                if (!GridData.TryGetUnit(gridComp, units, cell, out var unit))
                    continue;
                
                ref var unitComp = ref units.Get(unit);
                
                if(casterSide == unitComp.teamSide && !spell.canHitAllies)
                    return;
                    
                targets.Add(unit);

                if (!spell.isMultipleHit)
                    break;
            }
        }
        
        private static void CollectAreaTarget(GridComponent gridComp, Stash<CellTileComponent> tiles, Stash<UnitComponent> units,
            SpellConfigSo spell,
            CellCoord center,
            List<Entity> targets,
            TeamSide casterSide) //todo перепроверить
        {
            var radius = spell.areaRadius;
 
            for (var dq = -radius; dq <= radius; dq++)
            {
                var rMin = Math.Max(-radius, -dq - radius);
                var rMax = Math.Min(radius, -dq + radius);
 
                for (var dr = rMin; dr <= rMax; dr++)
                {
                    var cell = new CellCoord(center.Q + dq, center.R + dr);

                    if (!GridData.TryGetUnit(gridComp, units, cell, out var unit))
                        return;
                    
                    ref var unitComp = ref units.Get(unit);
                    
                    if(casterSide == unitComp.teamSide && !spell.canHitAllies)
                        continue;
 
                    targets.Add(unit);
                }
            }
        }

        private static bool TryLineTarget(GridComponent gridComp, Stash<CellTileComponent> tiles,
            CellCoord from, CellCoord to, SpellConfigSo spell,
            out CellCoord direction)
        {
            direction = default;

            foreach (var dir in HexMath.Directions)
            {
                var cell = from;

                for (int i = 0; i < spell.range; i++)
                {
                    cell += dir;
                    
                    if (cell == to)
                    {
                        direction = dir;
                        return true;
                    }

                    if (!GridData.IsFreeCell(gridComp, tiles, cell))
                        return false;
                    
                    if (!spell.isMultipleHit)
                        break;
                }
            }

            return false;
        }

        private static CellCoord GetDirection(CellCoord from, CellCoord to) => from.DirectionTo(to);
    }
}
