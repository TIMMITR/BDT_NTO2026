using System.Collections.Generic;
using Features.CharacterPack.SO;
using Features.LevelPack.SO;
using UnityEngine;

namespace Scenes.Game
{
    public class GameContext : MonoBehaviour
    {
        public GameObject boardGameObject;
        public LevelConfig levelConfig;
        public List<CharacterTemplate> characters;
    }
}