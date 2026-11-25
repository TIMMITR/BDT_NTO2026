using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.FightPack.Requests
{
    public struct AttackCommandRequest : IRequestData // todo переименовать
    {
        public Entity unit;
        public CellCoord target;
 
        public AttackCommandRequest(Entity unit, CellCoord target)
        {
            this.unit = unit;
            this.target = target;
        }
    }
}
