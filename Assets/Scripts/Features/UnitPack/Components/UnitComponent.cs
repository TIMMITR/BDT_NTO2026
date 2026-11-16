using Configs;
using Scellecs.Morpeh;

namespace Features.UnitPack.Components
{
    public struct UnitComponent : IComponent
    {
        public string characterName;
        public UnitClass unitClass;
        public TeamSide teamSide;

        public UnitComponent(string characterName, UnitClass unitClass, TeamSide teamSide)
        {
            this.characterName = characterName;
            this.unitClass = unitClass;
            this.teamSide = teamSide;
        }
    }
}
