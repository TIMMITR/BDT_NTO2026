using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public struct SpeedRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }
}
