using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.FightPack.Requests
{
    public struct AttackOrderRequest : IRequestData
    {
        public Entity source; // todo переименовать
        public Entity target;
        public CellCoord origin;

        public AttackOrderRequest(Entity source, Entity target, CellCoord origin)
        {
            this.source = source;
            this.target = target;
            this.origin = origin;
        }
    }
}
