using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "WorldSettings", menuName = "Configs/New World Settings", order = 2)]

    public class WorldSettingsSo : ScriptableObject
    {
        public int playerMaxMana = 10;// todo вынести в levelPlan
        // todo тут хранить данные 
    }
}