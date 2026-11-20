using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig_", menuName = "Enemy/New Enemy Config")]
    public class EnemyConfigSo : ScriptableObject
    {
        [SerializeField] private int attackWeight;
        [SerializeField] private int attackReduceValue;
        [SerializeField] private int captureWeight;
        [SerializeField] private int captureReduceValue;

        public int AttackWeight => attackWeight;
        public int AttackReduceValue => attackReduceValue;
        public int CaptureWeight => captureWeight;
        public int CaptureReduceValue => captureReduceValue;
    }
}
