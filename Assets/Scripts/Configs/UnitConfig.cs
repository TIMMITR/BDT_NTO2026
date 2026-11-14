using System;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "UnitConfigs", menuName = "Config/Unit Config", order = 7)]
    public sealed class UnitConfig : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private UnitClass unitClass;
        [SerializeField] private GameObject prefab;
        [SerializeField] private UnitStats stats = new()
        {
            maxHealth = 10,
            damage = 10,
            defence = 10,
            moveSpeed = 4,
            teamSide = TeamSide.Player
        };
        
        public string Name => characterName;
        public UnitClass Class => unitClass;
        public GameObject Prefab => prefab;
        public UnitStats Stats => stats;
    }
    
    [Serializable]
    public struct UnitStats
    {
        public int maxHealth;
        public int damage;
        public int defence;
        public int moveSpeed;
        public TeamSide teamSide;
        
    }
    
    public enum UnitClass
    {
        // под управлением игрока
        Paladin, 
        Wizard,
        // под управлением противника
        Skeleton, 
        Ranger,
        Warlock, // сам колдун если будем делать
    }
    
    public enum TeamSide
    {
        Player,
        Enemy
    }
}
