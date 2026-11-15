using System.Collections.Generic;
using Features.CellGridPack.Components;
using Features.HexGridPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.PathFinderPack
{
    public static class PathFinder
    {
        public static CellCoord[] Directions =
        {
            new(1,0),
            new(1, -1),
            new(0, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, 1),
        };
        
        public static PassReady GetPath(CellGridComponent grid, CellCoord from, CellCoord to, Stash<CellTileComponent> stash)
        {
            PassReady result = new() 
            {
                path = new Dictionary < CellCoord, CellCoord>(),
                cellsCost = new Dictionary<CellCoord, int>(),
            };

            result.cellsCost[from] = 0;
            result.path[from] = from;
            
            List<CostCoord> candidates = new()
            {
                new(from, 0),
            };

            while (candidates.Count > 0)
            {
                var currentCell = GetMin(candidates);

                foreach (var dir in Directions)
                {
                    var next = new CellCoord(currentCell.coord.Q + dir.Q, currentCell.coord.R + dir.R);

                    if (!grid.cells.TryGetValue(next, out var cell))
                        continue;

                    int cost = 0;

                    if (!CanPass(cell, stash, out cost)) 
                        continue;

                    if (cost <= 0)
                    {
                        Debug.Log("Too small cost");
                        continue;
                    }
                    
                    if (grid.takenCells.ContainsKey(next))
                        continue;
                    
                    int newCost = result.cellsCost[currentCell.coord] + cost;
                    
                    if (result.cellsCost.TryGetValue(next, out var exist) && newCost >= exist)
                        continue;
                    
                    result.cellsCost[next] = newCost;
                    result.path[next] = currentCell.coord;
                    candidates.Add(new CostCoord(next, newCost));
                }
            }
            
            return result;
        }

        public static List<CellCoord> ReconstructPath(PassReady pass, CellCoord from, CellCoord to)
        {
            List<CellCoord> result = new();
            
            if(!pass.path.ContainsKey(to)) return result;

            for (var i = to; i != from; i = pass.path[i])
            {
                result.Add(i);
            }
            
            result.Reverse();
            
            return result;
        }

        private static bool CanPass(Entity cell, Stash<CellTileComponent> stash, out int cost)
        {
            ref var cellComp = ref stash.Get(cell);
            cost = cellComp.moveCost;
            return !cellComp.blocked;
        }

        private static CostCoord GetMin(List<CostCoord> candidates)
        {
            int minIndex = 0;

            for (int i = 1; i < candidates.Count; i++)
            {
                if(candidates[i].cost < candidates[minIndex].cost)
                {
                    minIndex = i;
                }
            }
            
            var minCellCoord = candidates[minIndex];
            candidates.RemoveAt(minIndex);
            return minCellCoord;
        }
    }

    public struct PassReady
    {
        public Dictionary<CellCoord, CellCoord> path;
        public Dictionary<CellCoord, int> cellsCost;
    }
    
    public struct CostCoord // todo переименовать
    {
        public CellCoord coord;
        public int cost;
        
        public CostCoord(CellCoord coord, int cost)
        {
            this.coord = coord;
            this.cost = cost;
        }
    }
}

