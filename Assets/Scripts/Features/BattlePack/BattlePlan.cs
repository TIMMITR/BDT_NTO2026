using System;
using System.Collections.Generic;
using Configs;
using Features.QuickTalkPack.Data;
//using Features.QuickTalkPack.Data;
using UnityEngine;

namespace Features.BattlePack
{
    [CreateAssetMenu(fileName = "BattlePlan", menuName = "Plan/Battle Plan", order = 6)]
    public class BattlePlan : ScriptableObject
    {
        [SerializeField] private List<BattleTileData> units = new()
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

        [SerializeField] private List<ControlPointTileData> controlpoints = new();

        [SerializeField] private List<QuickTalkSet> quickTalks = new();

        public IReadOnlyList<BattleTileData> Units => units;
        public IReadOnlyList<ControlPointTileData> Controlpoints => controlpoints;
        public IReadOnlyList<QuickTalkSet> QuickTalks => quickTalks;
    }
    
    [Serializable]
    public struct BattleTileData
    {
        public int q;
        public int r;
        public int moveCost;
        public bool blocked; // если это препятствие
        public UnitConfig unitConfig; // может быть пустым
    }

    [Serializable]
    public struct ControlPointTileData
    {
        public int q;
        public int r;
        public ControlPointConfig config;
    }
    
    [Serializable]
    public struct ControlPointConfig
    {
        public GameObject prefab;
    }
}
