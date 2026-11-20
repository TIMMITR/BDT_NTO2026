using System.Collections.Generic;
using Configs;
using Features.GridPack.Data;
using JetBrains.Annotations;
using Scellecs.Morpeh;

namespace Features.FightPack.Requests
{
    public struct ProjectileCastRequest : IRequestData
    {
        public CellCoord from;
        public CellCoord to;
        public SpellConfigSo spell;
        public List<AttackOrderRequest> orders;

        public ProjectileCastRequest(CellCoord from, CellCoord to, SpellConfigSo spell,
            List<AttackOrderRequest> orders)
        {
            this.from = from;
            this.to = to;
            this.spell = spell;
            this.orders = orders;
        }
    }
}
