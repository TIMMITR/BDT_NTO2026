using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig_", menuName = "Enemy/New Enemy Config")]
    public class EnemyConfigSo : ScriptableObject
    {
        [SerializeField] private float attackWeight; // todo убрать или оставить
        [SerializeField] private float attackReduceValue;
        [SerializeField] private float attackIncreaseValue;
        [SerializeField] private float captureWeight;
        [SerializeField] private float captureReduceValue;
        [SerializeField] private float captureIncreaseValue;
        [SerializeField] private float defenceWeight;
        [SerializeField] private float defenceReduceValue;
        [SerializeField] private float defenceIncreaseValue;
        [SerializeField] private int defenceRadius;
        

        public float AttackWeight => attackWeight;
        public float AttackReduceValue => attackReduceValue;
        public float AttackIncreaseValue => attackIncreaseValue;
        public float CaptureWeight => captureWeight;
        public float CaptureReduceValue => captureReduceValue;
        public float CaptureIncreaseValue => captureIncreaseValue;
        public float DefenceWeight => defenceWeight;
        public float DefenceReduceValue => defenceReduceValue;
        public float DefenceIncreaseValue => defenceIncreaseValue;
        public int DefenceRadius => defenceRadius;
    }
}
