using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public struct HealthGiveRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }

    public struct HealthTakeRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }
    
}