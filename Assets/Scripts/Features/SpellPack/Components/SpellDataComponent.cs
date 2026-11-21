using Configs;
using Scellecs.Morpeh;

namespace Features.SpellPack.Components
{
    public struct SpellDataComponent : IComponent
    {
        public SpellConfigSo spellData;

        public SpellDataComponent(SpellConfigSo spellData)
        {
            this.spellData = spellData;
        }
    }
}
