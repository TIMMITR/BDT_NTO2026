using System;
using System.Collections.Generic;
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
            multiplier = 10,
            defence = 10,
            moveSpeed = 4,
        };

        [SerializeField] private float posY;
        [HideInInspector, SerializeField] private SpellConfigSo spellConfig;
        [SerializeField] private SpellConfigSo[] spells;
        
        public string Name => characterName;
        public UnitClass Class => unitClass;
        public GameObject Prefab => prefab;
        public UnitStats Stats => stats;
        public TeamSide TeamSide => teamSide;
        public float PosY => posY;
        public SpellConfigSo SpellConfig => spellConfig;
        public IReadOnlyList<SpellConfigSo> Spells
        {
            get
            {
                if (spells != null && spells.Length > 0)
                    return spells;

                return spellConfig != null
                    ? new[] { spellConfig }
                    : Array.Empty<SpellConfigSo>();
            }
        }
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
        Warlock
    }
    
    public enum TeamSide
    {
        Player,
        Enemy,
        None
    }
}
