using System.Collections.Generic;
using UnityEngine;

namespace Features.LevelPack.SO
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Level/Level Config", order = 52)]
    public sealed class LevelConfig : ScriptableObject
    {
        [SerializeField] private int gridRadius = 5;
        [SerializeField] private float cellRadius = 2f;
        [SerializeField] private int defaultMoveCost = 1;
        [SerializeField] private List<LevelCellData> customCells = new();
        
        public int GridRadius => gridRadius;
        public float CellRadius => cellRadius;
        public int DefaultMoveCost => defaultMoveCost;
        public IReadOnlyList<LevelCellData> CustomCells => customCells;
    }
}