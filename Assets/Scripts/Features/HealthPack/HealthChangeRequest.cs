using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public struct HealthChangeRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }
}
