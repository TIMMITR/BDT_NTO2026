using System;
using UnityEngine;

namespace Features.HexGridPack.Data
{
    // Замена кортежам, т.к. кортежи не очень читаемы,
    // Этот класс также включает методы перевода координат из мировых в локальные
    public readonly struct CellCoord : IEquatable<CellCoord>
    {
        public int Q { get; }
        public int R { get; }

        private const float CellTransMultiplier1 = 1.5f; // todo вынести в настройки
        private const float CellTransMultiplier2 = 0.5f;

        public CellCoord(int q, int r)
        {
            Q = q;
            R = r;
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
            var deltaQ = Q - hex.Q; 
            var deltaR = R - hex.R;
            
            return (Mathf.Abs(deltaQ) + Mathf.Abs(deltaR) + Mathf.Abs(deltaQ + deltaR)) / 2;
        }
        
        public Vector3 ToLocalPosition(float cellRadius) =>
            new(cellRadius * CellTransMultiplier1 * Q,
                0f,
                cellRadius * Mathf.Sqrt(3f) * (R + Q * CellTransMultiplier2));
        
        public static CellCoord FromLocalPosition(Vector3 local, float cellRadius)
        {
            var transQ = local.x / (cellRadius * CellTransMultiplier1);
            var transR = (local.z / (cellRadius * Mathf.Sqrt(3f))) - transQ * CellTransMultiplier2;
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
        
        public bool Equals(CellCoord other)
        {
            return Q == other.Q && R == other.R;
        }

        public override bool Equals(object obj)
        {
            return obj is CellCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Q, R);
        }

        public static bool operator ==(CellCoord left, CellCoord right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CellCoord left, CellCoord right)
        {
            return !left.Equals(right);
        }
    }
}