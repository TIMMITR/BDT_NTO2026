using Scellecs.Morpeh;

namespace Features.DamagePack
{
    public struct DamageRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }
}