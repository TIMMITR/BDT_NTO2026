using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public struct HealthRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }
}
