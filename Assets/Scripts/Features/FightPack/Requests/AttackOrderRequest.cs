using Configs;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.FightPack.Requests
{
    public struct AttackOrderRequest : IRequestData
    {
        public Entity source;
        public Entity target;
        public CellCoord origin;
        public SpellConfigSo spell;

        public AttackOrderRequest(Entity source, Entity target, CellCoord origin, SpellConfigSo spell)
        {
            this.source = source;
            this.target = target;
            this.origin = origin;
            this.spell = spell;
        }
    }
}
