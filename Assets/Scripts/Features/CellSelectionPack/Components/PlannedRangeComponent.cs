using System.Collections.Generic;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Components
{
    public struct PlannedRangeComponent : IComponent
    {
        public List<CellCoord> range;

        public PlannedRangeComponent(List<CellCoord> range)
        {
            this.range = range;
        }
    }
}
