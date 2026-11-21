using Scellecs.Morpeh;

namespace Features.FightPack.Events
{
    public struct UnitAttackedEvent : IEventData
    {
        public Entity source;
        public Entity target;

        public UnitAttackedEvent(Entity source, Entity target)
        {
            this.source = source;
            this.target = target;
        }
    }
}
