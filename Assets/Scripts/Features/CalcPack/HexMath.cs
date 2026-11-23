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
        
        public static IEnumerable<CellCoord> GetNeighbors(CellCoord center)
        {
            foreach (var dir in Directions)
                yield return center + dir;
        }
    }
}
