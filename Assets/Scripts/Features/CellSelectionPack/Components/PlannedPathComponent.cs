using System.Collections.Generic;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Components
{
    public struct PlannedPathComponent : IComponent
    {
        public List<CellCoord> path;

        public PlannedPathComponent(List<CellCoord> path)
        {
            this.path = path;
        }
    }
}
