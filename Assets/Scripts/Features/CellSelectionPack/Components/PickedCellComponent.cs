using System.Collections.Generic;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Components
{
    public struct PickedCellComponent : IComponent
    {
        public CellCoord cellCoord;
        public int moveSpeed;
        public List<CellCoord> points;

        public PickedCellComponent(CellCoord cellCoord, int moveSpeed)
        {
            this.cellCoord = cellCoord;
            this.moveSpeed = moveSpeed;
            points = new List<CellCoord>();
        }
    }
}