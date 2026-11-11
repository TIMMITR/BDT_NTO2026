using Features.CharacterPack.SO;
using Scellecs.Morpeh;

namespace Features.CharacterPack.Components
{
    public struct CharacterStatsComponent : IComponent
    {
        public CharacterClass characterClass; // todo вынести в отдельный компонент
        public CharacterStats stats;
    }
}
