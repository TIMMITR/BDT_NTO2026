using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;

namespace Features.GamePack
{
    [CreateAssetMenu(fileName = "GamePlan", menuName = "Plan/Game Plan", order = 5)]
    
    public class GamePlan : ScriptableObject
    {
        [SerializeField] private List<BattlePlan> battlePlans = new();
        
        public List<BattlePlan> BattlePlans => battlePlans;
    }
    
    [CreateAssetMenu(fileName = "BattlePlan", menuName = "Plan/Battle Plan", order = 6)]
    
    public class BattlePlan : ScriptableObject
    {
        [SerializeField] private List<BattleTileData> hexTiles = new();
        
        public IReadOnlyList<BattleTileData> HexTiles => hexTiles;
    }
    
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