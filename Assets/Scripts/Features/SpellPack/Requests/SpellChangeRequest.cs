using Scellecs.Morpeh;

namespace Features.SpellPack.Requests
{
    public struct SpellChangeRequest : IRequestData
    {
        public int index;

        public SpellChangeRequest(int index)
        {
            this.index = index;
        }
    }
}
