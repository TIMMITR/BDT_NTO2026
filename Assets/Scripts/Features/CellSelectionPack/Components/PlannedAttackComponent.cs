using System.Collections.Generic;
using Configs;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Components
{
    public struct PlannedAttackComponent : IComponent
    {
        public CellCoord target;
        public SpellConfigSo spell;
        public List<CellCoord> cells;
 
        public PlannedAttackComponent(CellCoord target, SpellConfigSo spell, List<CellCoord> cells)
        {
            this.target = target;
            this.spell = spell;
            this.cells = cells;
        }
    }
}
