using System.Collections.Generic;
using Features.CharacterPack.Components;
using Features.CharacterPack.SO;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CharacterPack.Systems
{
    public class CharactersInitSystem : IInitializer
    {
        public World World { get; set; }
 
        private Stash<CharacterStatsComponent> _characterStatsStash;
        private Stash<CharactersDataComponent> _charactersDataStash;
 
        private readonly List<CharacterTemplate> _characterTemplates;
 
        public CharactersInitSystem(List<CharacterTemplate> characterTemplates)
        {
            _characterTemplates = characterTemplates;
        }
 
        public void OnAwake()
        {
            _characterStatsStash = World.GetStash<CharacterStatsComponent>();
            _charactersDataStash = World.GetStash<CharactersDataComponent>();
 
            CreateCharacters();
        }
 
        private void CreateCharacters()
        {
            if (_characterTemplates == null)
            {
                Debug.Log("[CHARACTER_INIT_SYSTEM] Character templates has not been initialized.");
                return;
            }
            
            var characterDataEntity = World.CreateEntity();
            ref var characterDataComp = ref _charactersDataStash.Add(characterDataEntity);

            foreach (var template in _characterTemplates)
            {
                if (!template)
                {
                    Debug.Log("[CHARACTER_INIT_SYSTEM] Character template is null.");
                    continue;
                }

                var entity = World.CreateEntity();
                
                ref var characterComp = ref _characterStatsStash.Add(entity);
                
                characterComp.characterClass = template.Class;
                characterComp.stats = template.Stats;
                
                characterDataComp.characters.Add(characterComp.characterClass, entity); // todo возможно, стоит вынести в отдельную систему
            }
        }
 
        public void Dispose() {}
    }
}
