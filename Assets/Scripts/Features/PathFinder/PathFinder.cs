using System.Collections.Generic;
using Features.CellGridPack.Components;
using Features.HexGridPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.PathFinder
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
        
        public static void GetPath(CellGridComponent grid, CellCoord from, CellCoord to, Stash<CellTileComponent> stash)
        {
            Dictionary<CellCoord, int> cellsCost = new();

            cellsCost[from] = 0;
            
            List<CostCoord> candidates = new()
            {
                new(from, 0),
            };

            while (candidates.Count > 0)
            {
                var current = GetMin(candidates);

                foreach (var dir in Directions)
                {
                    var next = new CellCoord(current.coord.Q + dir.Q, current.coord.R + dir.R);

                    if (!grid.cells.TryGetValue(next, out var cell))
                        continue;

                    int cost = GetCost(cell, stash);

                    if (cost <= 0)
                    {
                        Debug.Log("Too small cost");
                        continue;
                    }
                    
                    if (grid.takenCells.ContainsKey(next))
                        continue;
                    
                    int newCost = cellsCost[current.coord] + cost;
                    
                    if (cellsCost.TryGetValue(next, out var exist) && newCost >= exist)
                        continue;
                    
                    cellsCost[next] = newCost;
                    candidates.Add(new CostCoord(next, newCost));
                }
            }
        }

        private static int GetCost(Entity cell, Stash<CellTileComponent> stash) => stash.Get(cell).moveCost;

        private static CostCoord GetMin(List<CostCoord> candidates)
        {
            return new CostCoord(new CellCoord(0, 0), 0);
        }
    }
    
    public class CostCoord // todo переименовать
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

