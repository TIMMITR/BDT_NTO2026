using Configs;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.FightPack.Components
{
    public struct AttackDesireComponent : IComponent
    {
        public CellCoord target;
        public SpellConfigSo spellConfig;

        public AttackDesireComponent(CellCoord target, SpellConfigSo spellConfig)
        {
            this.target = target;
            this.spellConfig = spellConfig;
        }
    }
}