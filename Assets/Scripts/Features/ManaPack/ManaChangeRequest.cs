using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public struct ManaChangeRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }
}
