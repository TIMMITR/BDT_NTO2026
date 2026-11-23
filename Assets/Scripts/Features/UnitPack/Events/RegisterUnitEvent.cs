using Scellecs.Morpeh;

namespace Features.UnitPack.Events
{
    public struct RegisterUnitEvent : IEventData
    {
        public Entity unit;
    }
}