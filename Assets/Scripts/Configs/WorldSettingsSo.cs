using System;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "WorldSettings", menuName = "Configs/New World Settings", order = 2)]
    public class WorldSettingsSo : ScriptableObject
    {
        public int playerMaxMana = 10; // todo вынести в levelPlan
        
        public int gridRadius = 5;
        public float cellRadius = 0.25f;
        public int defaultMoveCost = 1;

        [Header("Dialogue")]
        public float dialogueCharsPerSecond = 60f;
        public float dialogueStayTime = 3f;
        public float quickTalkGlobalCooldownSeconds = 15f;
        public float quickTalkCandidateTtlSeconds = 5f;

        public Vector2 rotationSpeedModifier = new(0.8f, 1.25f);
        public bool randomInitialRotation = true;
        public bool randomRotationDirection;

        public Vector2 bobbingAmplitudeBonus = new(0.0f, 0.25f);
        public Vector2 bobbingSpeedModifier = new(0.85f, 1.15f);
        public bool randomizeBobbingPhase = true;
    }
    
}