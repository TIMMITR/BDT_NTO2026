using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;

namespace Features.BattlePack
{
    [CreateAssetMenu(fileName = "BattlePlan", menuName = "Plan/Battle Plan", order = 6)]
    public class BattlePlan : ScriptableObject
    {
        [SerializeField] private List<BattleTileData> hexTiles = new()
        {
            new BattleTileData
            {
                q = 0,
                r = 0,
                moveCost = 1,
                blocked = false,
                unitConfig = null
            }
        };
        
        public IReadOnlyList<BattleTileData> HexTiles => hexTiles;
    
        [Serializable]
        public struct BattleTileData
        {
            public int q;
            public int r;
            public int moveCost;
            public bool blocked; // true если UnitConfig не пустой
            public UnitConfig unitConfig; // может быть пустым
        }
    }
}
