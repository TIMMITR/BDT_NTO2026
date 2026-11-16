using System.Collections.Generic;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.UnitPack.Components
{
    public struct UnitAnimationComponent : IComponent
    {
        public List<CellCoord> points;
        public bool isTurning;
        public int index;
        public float progress;

        public UnitAnimationComponent(List<CellCoord> points)
        {
            this.points = points;
            
            isTurning = true;
            index = 1;
            progress = 0f;
        }
    }
}