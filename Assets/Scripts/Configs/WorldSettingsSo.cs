using System;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "WorldSettings", menuName = "Configs/New World Settings", order = 2)]
    public class WorldSettingsSo : ScriptableObject
    {
        public string gameSceneName = "GameScene";
        
        public int playerMaxMana = 10; // todo вынести в levelPlan
        
        public int gridRadius = 5;
        public float cellRadius = 0.25f;
        public int defaultMoveCost = 1;


        [Header("Dialogue")]
        public float dialogueCharsPerSecond = 60f;
        [Min(1)] public int voiceCharacterInterval = 3;
        public float dialogueStayTime = 3f;
        public float quickTalkGlobalCooldownSeconds = 15f;
        public float quickTalkCandidateTtlSeconds = 5f;

        public Vector2 rotationSpeedModifier = new(0.8f, 1.25f);
        public bool randomInitialRotation = true;
        public bool randomRotationDirection;

        public Vector2 bobbingAmplitudeBonus = new(0.0f, 0.25f);
        public Vector2 bobbingSpeedModifier = new(0.85f, 1.15f);
        public bool randomizeBobbingPhase = true;
        
        [Header("Color")]
        public ColorData colorData;

        public int cardsManaCost = 2;
        public int vireteManaCost = 6;
        public int vireteDamage = 2;
        public int vireteHealing = 2;
        
        public float audioPitchVariation = 0.05f;
    }

    [Serializable]
    public struct ColorData
    {
        public Color clickedColor;
        public Color chosenColor;
        public Color pathColor;
        public Color attackColor;
        public Color rangeColor;
    }
}