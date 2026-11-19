using Scellecs.Morpeh;

namespace Features.DamagePack
{
    public struct DamageRequest : IRequestData
    {
        public Entity source; // может быть пригодится
        public Entity target;
        public int amount;

        public DamageRequest(Entity source, Entity target, int amount)
        {
            this.source = source;
            this.target = target;
            this.amount = amount;
        }
    }
}
