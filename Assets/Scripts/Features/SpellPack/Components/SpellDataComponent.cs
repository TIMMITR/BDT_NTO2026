using System;
using System.Collections.Generic;
using Configs;
using Scellecs.Morpeh;

namespace Features.SpellPack.Components
{
    public struct SpellDataComponent : IComponent
    {
        public SpellConfigSo spellData;
        public IReadOnlyList<SpellConfigSo> availableSpells;
        public int selectedSpellIndex;

        public SpellDataComponent(SpellConfigSo spellData)
            : this(spellData != null ? new[] { spellData } : Array.Empty<SpellConfigSo>())
        {
        }

        public SpellDataComponent(IReadOnlyList<SpellConfigSo> spells)
        {
            availableSpells = spells ?? Array.Empty<SpellConfigSo>();
            selectedSpellIndex = 0;
            spellData = availableSpells.Count > 0 ? availableSpells[0] : null;
        }
    }
}
