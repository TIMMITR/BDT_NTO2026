
using System;
using System.Collections.Generic;
using Features.GamePack;
using Features.StoryTalkPack.Data;
using UnityEngine;

namespace Scenes.Game
{
    public class GameContext : MonoBehaviour
    {
        public Transform playerSpawnPoint;

        [Header("Dialogue Settings")]
        public StoryTalk introTalk;

        [Header("Battle Settings")]
        public GamePlan gamePlan;
        
        [Header("Board Settings")]
        public GameObject boardGameObject;
        public Transform highlightCellPool;
        
        [Header("Unit Settings")]
        public List<MoveData> moveDataList;
    }

    [Serializable]
    public class MoveData
    {
        public float jumpHeight;
        public float moveDuration;
        public float rotateSpeed;
    }
}
