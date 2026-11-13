using System.Collections.Generic;
using Features.CharacterPack.SO;
using Features.LevelPack.SO;
using UnityEngine;

namespace Scenes.Game
{
    public class GameContext : MonoBehaviour
    {
        public Transform playerSpawnPoint;
        
        [Header("Board Settings")]
        public GameObject boardGameObject;
        public Transform highlightCellPool;
        public LevelConfig levelConfig;
        public List<CharacterTemplate> characters;
    }
}