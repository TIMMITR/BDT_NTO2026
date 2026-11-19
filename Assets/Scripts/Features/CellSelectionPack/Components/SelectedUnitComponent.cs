using System.Collections.Generic;
using Configs;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Components
{
    public struct SelectedUnitComponent : IComponent
    {
        public Entity unit;
        public TeamSide teamSide;
        public int moveSpeed;
        public List<CellCoord> path;

        public SelectedUnitComponent(Entity unit, TeamSide teamSide, int moveSpeed)
        {
            this.unit = unit;
            this.teamSide = teamSide;
            this.moveSpeed = moveSpeed;
            path = new List<CellCoord>();
        }

        public SelectedUnitComponent(Entity unit, TeamSide teamSide, int moveSpeed, List<CellCoord> path)
        {
            this.unit = unit;
            this.teamSide = teamSide;
            this.moveSpeed = moveSpeed;
            this.path = path;
        }
    }
}