using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public struct ManaChangeRequest : IRequestData
    {
        public int amount;
        public Entity entity;

        public ManaChangeRequest(int amount, Entity entity)
        {
            this.amount = amount;
            this.entity = entity;
        }
    }
}
