using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "SpellConfig_", menuName = "Spell/New Spell Config")]
    public class SpellConfigSo : ScriptableObject
    {
        [Header("UI")]
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private Sprite icon;

        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description;
        public Sprite Icon => icon;

        [Header("General")]
        public SpellType spellType;
        public int damage;
        public int range;
        public SpellRangeType rangeType;
        public int areaRadius; // только для удара по окружности
        public int manaCost;
        public bool ignoreArmor;
        public bool isMultipleHit; // todo не работает
        public bool canHitAllies;
        public bool strikeAtTarget; // todo посмотреть удар по прямой
 
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
