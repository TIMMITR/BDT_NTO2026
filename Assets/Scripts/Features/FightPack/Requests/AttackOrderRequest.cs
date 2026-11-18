using Scellecs.Morpeh;

namespace Features.FightPack.Requests
{
    public struct AttackOrderRequest : IRequestData
    {
        public Entity source; // todo переименовать
        public Entity target;

        public AttackOrderRequest(Entity source, Entity target)
        {
            this.source = source;
            this.target = target;
        }
    }
}
