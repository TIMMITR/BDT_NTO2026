using UnityEngine;
using UnityEngine.UI;

namespace Features.ItemsPack.Cards.Views
{
    public class CardsHolderView : MonoBehaviour
    {
        public Button cardsButton;
        public Image image;
        public Text cardName;
        public Text cardDescription;

        public void TurnCard(string cName, string description, Sprite sprite)
        {
            // todo сделать красивый переход
            cardName.text = cName;
            image.sprite = sprite;
            cardDescription.text = description;
            // если можно то primeTween'ом отключить легче
            // выключается через 3 секунды
        } 
    }
}