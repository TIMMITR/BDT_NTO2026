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
        public int areaRadius;
        public bool ignoreArmor;
        public bool isMultipleHit;
        public bool canHitAllies;
        public int punchDistance;
 
        [Header("Цепочка")]
        public SpellConfigSo nextSpell;
        public NextSpellOrigin nextSpellOrigin;
        public int nextSpellDelay; 
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
