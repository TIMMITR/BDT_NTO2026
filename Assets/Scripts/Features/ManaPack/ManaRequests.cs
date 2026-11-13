using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public struct ManaGiveRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }
    
    public struct ManaTakeRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }
}