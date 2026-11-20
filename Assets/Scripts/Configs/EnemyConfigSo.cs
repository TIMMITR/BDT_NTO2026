using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig_", menuName = "Enemy/New Enemy Config")]
    public class EnemyConfigSo : ScriptableObject
    {
        [SerializeField] private List<UnitConfig> crystalSpawnUnits = new();
        public List<UnitConfig> CrystalSpawnUnits => crystalSpawnUnits;
    }
}
