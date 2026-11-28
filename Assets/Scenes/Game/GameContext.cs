using System;
using System.Collections.Generic;
using Features.GamePack;
using Features.OverlayPack.Views;
using Features.StoryTalkPack.Data;
using Features.TutorialPack;
using UnityEngine;

namespace Scenes.Game
{
    public class GameContext : MonoBehaviour
    {
        public Transform playerSpawnPoint;

        [Header("Dialogue Settings")]
        public StoryTalk introTalk;
        public List<TutorialStep> tutorialSteps;

        [Header("Battle Settings")]
        public GamePlan gamePlan;

        [Header("UI Settings")]
        public BattleUiView battleUi;
        
        [Header("Board Settings")]
        public GameObject boardGameObject;
        public Transform highlightCellPool;

        [Header("Camera Settings")]
        public Transform cameraCorridorStart;
        public Transform cameraCorridorEnd;
        
        [Header("Unit Settings")]
        public List<AnimationData> moveDataList;
    }

    [Serializable]
    public class AnimationData
    {
        public float jumpHeight;
        public float moveDuration;
        public float rotateSpeed;
    }
}
  