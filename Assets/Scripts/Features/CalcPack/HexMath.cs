using System;
using System.Collections.Generic;
using Features.GridPack.Data;

namespace Features.CalcPack
{
    public static class HexMath
    {
        public static readonly CellCoord[] Directions =
        {
            new(1,0),
            new(1, -1),
            new(0, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, 1),
        };
        
        public static IEnumerable<CellCoord> GetNeighbours(CellCoord center)
        {
            foreach (var dir in Directions)
                yield return center + dir;
        }
        
        public static IEnumerable<CellCoord> GetRange(CellCoord center, int radius)
        {
            for (var dq = -radius; dq <= radius; dq++)
            {
                var rMin = Math.Max(-radius, -dq - radius);
                var rMax = Math.Min(radius, -dq + radius);
 
                for (var dr = rMin; dr <= rMax; dr++)
                    yield return new CellCoord(center.Q + dq, center.R + dr);
            }
        }

        public static IEnumerable<CellCoord> GetRing(CellCoord center, int radius)
        {
            if (radius <= 0)
            {
                yield return center;
                yield break;
            }

            var cell = center + Directions[4] * radius; // такой четкий индекс нужен для правильного обхода вокруг

            for (var side = 0; side < 6; side++)
            {
                for (var step = 0; step < radius; step++)
                {
                    yield return cell;
                    cell += Directions[side];
                }
            }
        }
    }
}
