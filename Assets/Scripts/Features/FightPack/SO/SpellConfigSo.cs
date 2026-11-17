using UnityEngine;

namespace Features.FightPack.SO
{
    public class SpellConfigSo : MonoBehaviour
    {
        [Header("General")]
        public SpellType spellType;
        public int damage;
        public int range;
        public SpellRangeType rangeType;
        public bool ignoreArmor;
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
