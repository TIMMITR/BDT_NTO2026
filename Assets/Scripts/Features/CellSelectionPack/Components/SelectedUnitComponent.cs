using System.Collections.Generic;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Components
{
    public struct SelectedUnitComponent : IComponent
    {
        public Entity unit;
        public int moveSpeed;
        public List<CellCoord> path;

        public SelectedUnitComponent(Entity unit, int moveSpeed)
        {
            this.unit = unit;
            this.moveSpeed = moveSpeed;
            path = new List<CellCoord>();
        }

        public SelectedUnitComponent(Entity unit, int moveSpeed, List<CellCoord> path)
        {
            this.unit = unit;
            this.moveSpeed = moveSpeed;
            this.path = path;
        }
    }
}