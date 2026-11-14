using System.Collections.Generic;
using Features.CharacterPack.SO;
using Scellecs.Morpeh;

namespace Features.CharacterPack.Components
{
    public struct CharactersDataComponent : IComponent
    {
        public Dictionary<CharacterClass, Entity> characters;

        public CharactersDataComponent(string str) // todo посмотреть, как можно убрать
        {
            characters = new ();
        }
    }
}
