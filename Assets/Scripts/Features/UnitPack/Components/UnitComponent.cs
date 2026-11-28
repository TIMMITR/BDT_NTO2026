using Configs;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.UnitPack.Components
{
    public struct UnitComponent : IComponent
    {
        public string characterName;
        public UnitClass unitClass;
        public Sprite unitImage;
        public TeamSide teamSide;
        public float posY;

        public UnitComponent(string characterName, UnitClass unitClass, TeamSide teamSide, float posY, Sprite unitImage)
        {
            this.characterName = characterName;
            this.unitClass = unitClass;
            this.teamSide = teamSide;
            this.posY = posY;
            this.unitImage = unitImage;
        }
    }
}
