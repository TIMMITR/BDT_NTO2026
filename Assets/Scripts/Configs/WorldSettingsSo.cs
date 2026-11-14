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
        // todo тут хранить данные 
    }
}