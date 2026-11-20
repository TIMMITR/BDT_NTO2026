using Configs;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.ControlPointPack.Components
{
    public struct ControlPointComponent : IComponent
    {
        public CellCoord cellCoord;
        public TeamSide teamSide;

        public ControlPointComponent(CellCoord cellCoord, TeamSide teamSide)
        {
            this.cellCoord = cellCoord;
            this.teamSide = teamSide;
        }
    }
}
