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
        [SerializeField] private float defenceWeight;
        [SerializeField] private float defenceReduceValue;
        [SerializeField] private int defenceRadius;

        public float AttackWeight => attackWeight;
        public float AttackReduceValue => attackReduceValue;
        public float CaptureWeight => captureWeight;
        public float CaptureReduceValue => captureReduceValue;
        public float DefenceWeight => defenceWeight;
        public float DefenceReduceValue => defenceReduceValue;
        public int DefenceRadius => defenceRadius;
    }
}
