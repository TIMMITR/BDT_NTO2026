using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "SpellConfig_", menuName = "Spell/New Spell Config")]
    public class SpellConfigSo : ScriptableObject
    {
        [Header("General")]
        public SpellType spellType;
        public int damage;
        public int range;
        public SpellRangeType rangeType;
        public int areaRadius; // только для удара по окружности
        public bool ignoreArmor;
        public bool isMultipleHit;
        public bool canHitAllies;
        public bool strikeAtTarget;
 
        [Header("Punch")]
        public int punchDistance;
        public int punchDamage;
        public int punchReturnDistance;
        
        [Header("Next Spell")]
        public SpellConfigSo nextSpell;
        public NextSpellOrigin nextSpellOrigin;
        public int nextSpellDelay; 
        
        [Header("Visual")]
        public GameObject projectilePrefab;
        public GameObject explosionPrefab;
        public float projectileSpeed;
        public float projectileHeight;
        public float swayAmplitude;
        public float explosionLifetime;
    }

    public enum SpellType
    {
        None,
    }

    public enum SpellRangeType
    {
        Straight,
        Area,
        Cone,
    }

    public enum NextSpellOrigin
    {
        FromUnit,
        AtPreviousTarget,
    }
}
