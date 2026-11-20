using Features.ControlPointPack.Components;
using Scellecs.Morpeh;

namespace Features.ControlPointPack.Events
{
    public struct ControlPointChangedEvent : IEventData
    {
        public Entity controlPoint;

        public ControlPointChangedEvent(Entity controlPoint)
        {
            this.controlPoint = controlPoint;
        }
    }
}
