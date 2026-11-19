using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.KnockbackPack.Requests
{
    public struct KnockbackRequest : IRequestData
    {
        public Entity source;
        public Entity unit;
        public CellCoord dir;
        public int distance;
        public int damage;
        public int returnDistance;

        public KnockbackRequest(Entity source, Entity unit, CellCoord dir, int distance, int damage, int returnDistance)
        {
            this.source = source;
            this.unit = unit;
            this.dir = dir;
            this.distance = distance;
            this.damage = damage;
            this.returnDistance = returnDistance;
        }
    }
}
