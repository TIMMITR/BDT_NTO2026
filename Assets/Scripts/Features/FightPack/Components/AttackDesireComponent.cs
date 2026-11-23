using Configs;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.FightPack.Components
{
    public struct AttackDesireComponent : IComponent
    {
        public CellCoord target;
        public SpellConfigSo spellConfig;
    }
}