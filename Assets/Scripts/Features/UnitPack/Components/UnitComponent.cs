using Configs;
using Scellecs.Morpeh;

namespace Features.UnitPack.Components
{
    public struct UnitComponent : IComponent
    {
        public string characterName;
        public UnitClass unitClass;
        public TeamSide teamSide;
        public float posY;

        public UnitComponent(string characterName, UnitClass unitClass, TeamSide teamSide, float posY)
        {
            this.characterName = characterName;
            this.unitClass = unitClass;
            this.teamSide = teamSide;
            this.posY = posY;
        }
    }
}
