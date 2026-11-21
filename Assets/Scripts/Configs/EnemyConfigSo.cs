using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig_", menuName = "Enemy/New Enemy Config")]
    public class EnemyConfigSo : ScriptableObject
    {
        [SerializeField] private float attackWeight;
        [SerializeField] private float attackReduceValue;
        [SerializeField] private float captureWeight;
        [SerializeField] private float captureReduceValue;

        public float AttackWeight => attackWeight;
        public float AttackReduceValue => attackReduceValue;
        public float CaptureWeight => captureWeight;
        public float CaptureReduceValue => captureReduceValue;
    }
}
