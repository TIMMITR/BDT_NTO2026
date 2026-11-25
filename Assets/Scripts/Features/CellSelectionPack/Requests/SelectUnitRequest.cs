using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Requests
{
    public struct SelectUnitRequest : IRequestData
    {
        public Entity unit;

        public SelectUnitRequest(Entity unit)
        {
            this.unit = unit;
        }
    }
}
