using Scellecs.Morpeh;

namespace Features.DamagePack
{
    public struct DamageRequest : IRequestData
    {
        public Entity source; // может быть пригодится
        public Entity target;
    }
}
