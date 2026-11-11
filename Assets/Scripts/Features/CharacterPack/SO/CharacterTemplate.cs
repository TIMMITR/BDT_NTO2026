using UnityEngine;

namespace Features.CharacterPack.SO
{
    [CreateAssetMenu(fileName = "CharacterTemplate", menuName = "Character/Character Template", order = 51)]
    public sealed class CharacterTemplate : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private CharacterStats stats = new()
        {
            maxHealth = 10, damage = 10, defence = 10, moveSpeed = 4
        };
        
        public string Name => characterName;
        public CharacterClass Class => characterClass;
        public CharacterStats Stats => stats;
    }
}
