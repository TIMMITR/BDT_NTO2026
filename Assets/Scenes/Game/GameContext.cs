
using UnityEngine;

namespace Scenes.Game
{
    public class GameContext : MonoBehaviour
    {
        public Transform playerSpawnPoint;
        
        [Header("Board Settings")]
        public GameObject boardGameObject;
        public Transform highlightCellPool;
    }
}
