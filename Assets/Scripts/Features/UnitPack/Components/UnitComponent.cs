using Configs;
using Scellecs.Morpeh;

namespace Features.UnitPack.Components
{
    public struct UnitComponent : IComponent
    {
        public string characterName;
        public UnitClass unitClass;

        public UnitComponent(string characterName, UnitClass unitClass)
        {
            this.characterName = characterName;
            this.unitClass = unitClass;
        }
    }
}
