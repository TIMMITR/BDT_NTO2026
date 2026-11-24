using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public struct ManaChangeRequest : IRequestData
    {
        public int amount;

        public ManaChangeRequest(int amount)
        {
            this.amount = amount;
        }
    }
}
