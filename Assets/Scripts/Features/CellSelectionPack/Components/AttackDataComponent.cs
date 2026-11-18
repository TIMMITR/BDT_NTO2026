using Configs;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Components
{
    public struct AttackDataComponent : IComponent
    {
        public SpellConfigSo spellConfig;
        public CellCoord from;
        public CellCoord to;
        public CellCoord dir;

        public AttackDataComponent(SpellConfigSo spell, CellCoord from, CellCoord to)
        {
            this.spellConfig = spell;
            this.from = from;
            this.to = to;
            dir = default;
        }
    }
}
