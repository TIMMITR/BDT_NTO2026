using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "WorldSettings", menuName = "Configs/New World Settings", order = 2)]

    public class WorldSettingsSo : ScriptableObject
    {
        public int playerMaxMana = 10;// todo вынести в levelPlan
        
        public int gridRadius = 5;
        public float cellRadius = 0.25f;
        public int defaultMoveCost = 1;

        [Header("Dialogue")]
        public float dialogueCharsPerSecond = 60f;
        public float dialogueStayTime = 3f;
        public float quickTalkGlobalCooldownSeconds = 15f; // минимум между любыми QuickTalk
        public float quickTalkCandidateTtlSeconds = 5f;    // через сколько кандидат протухает
        // todo тут хранить данные
    }
}