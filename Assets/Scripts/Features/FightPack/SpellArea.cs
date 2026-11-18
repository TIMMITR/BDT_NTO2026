using System;
using System.Collections.Generic;
using Configs;
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
            SpellConfigSo spell,
            CellCoord from, CellCoord to, CellCoord direction,
            TeamSide casterSide)
        {
            List<Entity> targets = new();

            switch (spell.rangeType)
            {
                case SpellRangeType.Straight:
                    CollectLineTarget(grid, tiles, units, spell, from, to, direction, targets, casterSide);
                    break;
                case SpellRangeType.Area:
                    CollectAreaTarget(grid, tiles, units, spell, from, targets, casterSide);
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
                    ReconstructArea(grid, spell, from, cells);
                    break;
                case SpellRangeType.Cone:
                    break;
            }

            return cells;
        }
        
        private static void ReconstructLine(GridComponent grid, Stash<CellTileComponent> tiles,
            SpellConfigSo spell,
            CellCoord from, CellCoord direction, List<CellCoord> cells)
        {
            var cell = from;
 
            for (var i = 0; i < spell.range; i++)
            {
                cell = new CellCoord(cell.Q + direction.Q, cell.R + direction.R);
 
                if (!grid.cells.TryGetValue(cell, out var cellEntity))
                    break;
 
                cells.Add(cell);            
 
                if (!spell.isMultipleHit && grid.takenCells.ContainsKey(cell))
                    break;
            }
        }
        
        private static void ReconstructArea(GridComponent grid, 
            SpellConfigSo spell,
            CellCoord from, List<CellCoord> cells)
        {
            for (var dq = -spell.range; dq <= spell.range; dq++)
            {
                var rMin = Math.Max(-spell.range, -dq - spell.range);
                var rMax = Math.Min(spell.range, -dq + spell.range);
 
                for (var dr = rMin; dr <= rMax; dr++)
                {
                    var cell = new CellCoord(from.Q + dq, from.R + dr);
                    
                    if (grid.cells.ContainsKey(cell))
                        cells.Add(cell);
                }
            }
        }

        private static void CollectLineTarget(GridComponent grid, Stash<CellTileComponent> tiles, Stash<UnitComponent> units,
            SpellConfigSo spell,
            CellCoord from, CellCoord to, CellCoord direction,
            List<Entity> targets,
            TeamSide casterSide) //todo перепроверить
        {
            for (int i = 0; i < spell.range; i++)
            {
                var cell = new CellCoord(from.Q + direction.Q, from.R + direction.R);

                if (!grid.cells.TryGetValue(cell, out var a)) 
                    break;
                
                if (!grid.takenCells.TryGetValue(cell, out var unit))
                    break;
                
                targets.Add(unit);

                if (!spell.isMultipleHit)
                    break;
            }
        }
        
        private static void CollectAreaTarget(GridComponent grid, Stash<CellTileComponent> tiles, Stash<UnitComponent> units,
            SpellConfigSo spell,
            CellCoord from,
            List<Entity> targets,
            TeamSide casterSide) //todo перепроверить
        {
            var range = spell.range;
 
            for (var dq = -range; dq <= range; dq++)
            {
                var rMin = Math.Max(-range, -dq - range);
                var rMax = Math.Min(range, -dq + range);
 
                for (var dr = rMin; dr <= rMax; dr++)
                {
                    var cell = new CellCoord(from.Q + dq, from.R + dr);
 
                    if (!grid.takenCells.TryGetValue(cell, out var unit)) 
                        continue;
                    if (units.Get(unit).teamSide == casterSide)
                        continue;
 
                    targets.Add(unit);
                }
            }
        }

        private static bool TryLineTarget(GridComponent grid, Stash<CellTileComponent> tiles,
            CellCoord from, CellCoord to, SpellConfigSo spell,
            out CellCoord direction)
        {
            direction = default;

            foreach (var dir in PathFinder.Directions)
            {
                var cell = from;

                for (int i = 0; i < spell.range; i++)
                {
                    cell = new CellCoord(cell.Q + dir.Q, cell.R + dir.R);
                    
                    if (cell == to)
                    {
                        direction = dir;
                        return true;
                    }

                    if (!grid.cells.TryGetValue(cell, out var gridCell))
                        break;
                    if (tiles.Get(gridCell).blocked)
                        break;
                    if (!spell.isMultipleHit && grid.takenCells.ContainsKey(cell))
                        break;
                }
            }

            return false;
        }

        private static CellCoord GetDirection(CellCoord from, CellCoord to)
        {
            var dq = to.Q - from.Q;
            var dr = to.R - from.R;
            var da = -dq - dr;
 
            if (Mathf.Abs(dq) >= Mathf.Abs(dr) && Mathf.Abs(dq) >= Mathf.Abs(da))
                return new CellCoord(Math.Sign(dq), 0);
 
            if (Mathf.Abs(dr) >= Mathf.Abs(da))
                return new CellCoord(0, Math.Sign(dr));
 
            return new CellCoord(-Math.Sign(da), Math.Sign(da));
        }
    }
}
