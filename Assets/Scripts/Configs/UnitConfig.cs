using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Configs
{
    [CreateAssetMenu(fileName = "UnitConfig_", menuName = "Unit/New Unit Config", order = 7)]
    public sealed class UnitConfig : ScriptableObject // todo надо решить как именовать поля
    {
        [SerializeField] private string characterName;
        [SerializeField] private UnitClass unitClass;
        [SerializeField] private Sprite unitImage;
        [SerializeField] private TeamSide teamSide;
        [SerializeField] private GameObject prefab;
        [SerializeField] private UnitStats stats = new()
        {
            maxHealth = 10,
            multiplier = 10,
            defence = 10,
            moveSpeed = 4,
        };

        [SerializeField] private float posY;
        [SerializeField] private List<SpellConfigSo> spellList;
        [SerializeField] private float spawnRotationY;
        
        public string Name => characterName;
        public UnitClass Class => unitClass;
        public Sprite UnitImage => unitImage;
        public GameObject Prefab => prefab;
        public UnitStats Stats => stats;
        public TeamSide TeamSide => teamSide;
        public float PosY => posY;
        public List<SpellConfigSo> SpellList => spellList;
        public float SpawnRotationY => spawnRotationY;
    }
    
    [Serializable]
    public struct UnitStats
    {
        public int maxHealth;
        public int multiplier;
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
        Warlock,
        // статичные объекты
        Base
    }
    
    public enum TeamSide
    {
        Player,
        Enemy,
        None
    }
}
