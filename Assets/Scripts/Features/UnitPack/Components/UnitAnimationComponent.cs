using System.Collections.Generic;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.UnitPack.Components
{
    public struct UnitAnimationComponent : IComponent
    {
        public List<CellCoord> points;
        public bool isSlide;
        
        public bool isTurning;
        public int index;
        public float progress;

        public UnitAnimationComponent(List<CellCoord> points, bool isSlide)
        {
            this.points = points;
            this.isSlide = isSlide;
            
            isTurning = true;
            index = 1;
            progress = 0f;
        }
    }
}