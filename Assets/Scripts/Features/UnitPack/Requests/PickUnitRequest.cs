using Scellecs.Morpeh;

namespace Features.UnitPack.Requests
{
    public struct PickUnitRequest : IRequestData
    {
        public Entity unit;

        public PickUnitRequest(Entity unit)
        {
            this.unit = unit;
        }
    }
}
