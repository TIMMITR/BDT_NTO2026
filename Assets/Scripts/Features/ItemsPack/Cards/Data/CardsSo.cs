using UnityEngine;

namespace Features.ItemsPack.Cards.Data
{
    [CreateAssetMenu(fileName = "Card", menuName = "Configs/Card", order = 67)]
    public class CardsSo : ScriptableObject
    {
        public CardsHolderData cardsHolderData;
    }
    
    [System.Serializable]
    public struct CardsHolderData
    {
        public string cardName;
        public string cardDescription;
        public CardType cardType;
        public Sprite cardUiSprite;
    }

    public enum CardType
    {
        CoolCard,
        HealingCard,
        DeadCard
    }
}