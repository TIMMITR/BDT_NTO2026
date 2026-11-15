using System.Collections.Generic;
using Features.HexGridPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellPack.Components
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