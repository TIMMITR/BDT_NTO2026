using Scellecs.Morpeh;

namespace Features.GridPack.Components
{
    public struct CellTileComponent : IComponent
    {
        public int q;
        public int r;
        public int moveCost;
        public bool blocked;

        public CellTileComponent(int q, int r, int moveCost, bool blocked)
        {
            this.q = q;
            this.r = r;
            this.moveCost = moveCost;
            this.blocked = blocked;
        }
    }
}