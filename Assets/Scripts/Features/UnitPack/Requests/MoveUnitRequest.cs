using System.Collections.Generic;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.UnitPack.Requests
{
    public struct MoveUnitRequest : IRequestData
    {
        public Entity unit;
        public List<CellCoord> path;

        public MoveUnitRequest(Entity unit, List<CellCoord> path)
        {
            this.unit = unit;
            this.path = path;
        }
    }
}