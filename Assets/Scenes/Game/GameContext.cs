using System;
using System.Collections.Generic;
using Features.GamePack;
using Features.OverlayPack.Views;
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

        [Header("Camera Settings")]
        public Transform cameraCorridorStart;
        public Transform cameraCorridorEnd;
        
        [Header("Unit Settings")]
        public List<AnimationData> moveDataList;

        [Header("Battle UI")]
        public HeroSlotView[] heroSlots = Array.Empty<HeroSlotView>();
        public SpellSlotView[] spellSlots = Array.Empty<SpellSlotView>();
    }

    [Serializable]
    public class AnimationData
    {
        public float jumpHeight;
        public float moveDuration;
        public float rotateSpeed;
    }
}
  