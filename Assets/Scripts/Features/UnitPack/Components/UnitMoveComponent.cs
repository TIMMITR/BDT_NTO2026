using System.Collections.Generic;
using Features.HexGridPack.Data;
using Scellecs.Morpeh;

namespace Features.UnitPack.Components
{
    public struct UnitMoveComponent : IComponent
    {
        public List<CellCoord> points;
        public bool isTurning;
        public int index;
        public float percent;

        public UnitMoveComponent(List<CellCoord> points)
        {
            this.points = points;
            
            isTurning = true;
            index = 1;
            percent = 0f;
        }
    }
}