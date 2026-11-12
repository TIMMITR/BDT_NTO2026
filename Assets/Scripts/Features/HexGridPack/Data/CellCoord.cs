using System;
using UnityEngine;

namespace Features.HexGridPack.Data
{
    // Замена кортежам, т.к. кортежи не очень читаемы,
    // Этот класс также включает методы перевода координат из мировых в локальные
    public struct CellCoord
    {
        public int q => _q;
        public int r => _r;
        
        private int _q;
        private int _r;
        
        private const float CELL_TRANS_MULTIPLIER_1 = 1.5f; // todo вынести в настройки
        private const float CELL_TRANS_MULTIPLIER_2 = 0.5f;

        public CellCoord(int q, int r)
        {
            _q = q;
            _r = r;
        }
 
        public static readonly CellCoord[] Directions =
        {
            new(1, 0),
            new(1, -1),
            new(0, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, 1)
        };
 
        public int DistanceTo(CellCoord hex)
        {
            var deltaQ = q - hex.q; 
            var deltaR = r - hex.r;
            
            return (Mathf.Abs(deltaQ) + Mathf.Abs(deltaR) + Mathf.Abs(deltaQ + deltaR)) / 2;
        }
        
        public Vector3 ToLocalPosition(float cellRadius) => new(cellRadius * CELL_TRANS_MULTIPLIER_1 * _q, 0f, cellRadius * Mathf.Sqrt(3f) * (_r + _q * CELL_TRANS_MULTIPLIER_2));
        
        public static CellCoord FromLocalPosition(Vector3 local, float cellRadius)
        {
            var transQ = local.x / (cellRadius * CELL_TRANS_MULTIPLIER_1);
            var transR = (local.z / (cellRadius * Mathf.Sqrt(3f))) - transQ * CELL_TRANS_MULTIPLIER_2;
            return Round(transQ, transR);
        }
        
        private static CellCoord Round(float q, float r)
        {
            var a = -q - r;
 
            var rq = Mathf.RoundToInt(q);
            var rr = Mathf.RoundToInt(r);
            var ra = Mathf.RoundToInt(a);
 
            var deltaQ = Mathf.Abs(rq - q);
            var deltaR = Mathf.Abs(rr - r);
            var deltaA = Mathf.Abs(ra - a);
 
            if (deltaQ > deltaR && deltaQ > deltaA)
                rq = -rr - ra;
            else if (deltaR > deltaA)
                rr = -rq - ra;
 
            return new CellCoord(rq, rr);
        }
        
        public bool Equals(CellCoord other) => q == other.q && r == other.r;
    }
}