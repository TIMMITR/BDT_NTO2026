using System;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Configs", menuName = "Config/Unit Config", order = 7)]
    public sealed class UnitConfig : ScriptableObject // todo надо решить как именовать поля
    {
        [SerializeField] private string characterName;
        [SerializeField] private UnitClass unitClass;
        [SerializeField] private TeamSide teamSide;
        [SerializeField] private GameObject prefab;
        [SerializeField] private UnitStats stats = new()
        {
            maxHealth = 10,
            damage = 10,
            defence = 10,
            moveSpeed = 4,
        };
        
        public string Name => characterName;
        public UnitClass Class => unitClass;
        public GameObject Prefab => prefab;
        public UnitStats Stats => stats;
        public TeamSide TeamSide => teamSide;
    }
    
    [Serializable]
    public struct UnitStats
    {
        public int maxHealth;
        public int damage;
        public int defence;
        public int moveSpeed;
    }
    
    public enum UnitClass
    {
        // под управлением игрока
        Paladin, 
        Wizard,
        // под управлением противника
        Skeleton, 
        Ranger,
        Warlock
    }
    
    public enum TeamSide
    {
        Player,
        Enemy
    }
}
