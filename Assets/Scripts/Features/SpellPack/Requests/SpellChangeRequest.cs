using Scellecs.Morpeh;

namespace Features.SpellPack.Requests
{
    public struct SpellChangeRequest : IRequestData
    {
        public Entity unit;
        public int spellIndex;

        public SpellChangeRequest(Entity unit, int spellIndex)
        {
            this.unit = unit;
            this.spellIndex = spellIndex;
        }
    }
}
