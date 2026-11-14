
using Features.GamePack;
using UnityEngine;

namespace Scenes.Game
{
    public class GameContext : MonoBehaviour
    {
        public Transform playerSpawnPoint;
        
        [Header("Battle Settings")]
        public GamePlan gamePlan;
        
        [Header("Board Settings")]
        public GameObject boardGameObject;
        public Transform highlightCellPool;
    }
}
