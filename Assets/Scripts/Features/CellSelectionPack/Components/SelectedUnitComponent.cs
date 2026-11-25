using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Components
{
    public struct SelectedUnitComponent : IComponent
    {
        public Entity unit;

        public SelectedUnitComponent(Entity unit)
        {
            this.unit = unit;
        }
    }
}