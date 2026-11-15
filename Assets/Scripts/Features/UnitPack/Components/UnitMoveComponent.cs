using System.Collections.Generic;
using Features.HexGridPack.Data;
using Scellecs.Morpeh;

namespace Features.UnitPack.Components
{
    public struct UnitMoveComponent : IComponent
    {
        public List<CellCoord> points;
        public bool isTurning;
        public float duration;
        public float jumpHeight; // todo вынести в контекст
        public int index;
        public float percent;

        public UnitMoveComponent(List<CellCoord> points, float duration, float jumpHeight)
        {
            this.points = points;
            this.duration = duration;
            this.jumpHeight = jumpHeight;
            
            isTurning = true;
            index = 1;
            percent = 0f;
            duration = 1f;
        }
    }
}
