using System;

namespace Features.LevelPack.SO
{
    [Serializable]
    public struct LevelCellData
    {
        public int q;
        public int r;
        public bool blocked;
        public int moveCost;
    }
}