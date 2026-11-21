using System.Collections.Generic;
using Configs;
using Scellecs.Morpeh;

namespace Features.SpellPack.Components
{
    public struct SpellListComponent : IComponent
    {
        public List<SpellConfigSo> spellList;
        public int currentIndex;

        public SpellListComponent(List<SpellConfigSo> spellList)
        {
            this.spellList = spellList;
            currentIndex = -1;
        }
    }
}
