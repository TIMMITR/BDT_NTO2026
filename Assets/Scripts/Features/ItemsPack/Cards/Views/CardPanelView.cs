using System.Collections.Generic;
using Features.ItemsPack.Cards.Data;
using Features.ItemsPack.Cards.Events;
using Features.ItemsPack.Cards.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.ItemsPack.Cards.Views
{
    public class CardPanelView : MonoBehaviour
    {
        public List<CardsHolderView> cardsHolderViews;
        private List<CardsHolderData> _shownCards = new();
        private bool _isCardPicked = true;

        private void Awake()
        {
            for (var i = 0; i < cardsHolderViews.Count; i++)
            {
                var index = i;
                cardsHolderViews[i].HideImmediately();
                cardsHolderViews[i].cardsButton.onClick.AddListener(() => PickCard(index));
            }

            World.Default.GetEvent<ShowCardsEvent>().Subscribe(changes =>
            {
                foreach (var change in changes)
                {
                    ShowCards(change.cardsHolderDatas);
                }
            });
        }

        private void ShowCards(List<CardsHolderData> cards)
        {
            for (var i = 0; i < cardsHolderViews.Count; i++)
            {
                cardsHolderViews[i].HideImmediately();
            }

            _shownCards = cards;
            _isCardPicked = false;

            for (var i = 0; i < cardsHolderViews.Count && i < cards.Count; i++)
            {
                var cardsHolderView = cardsHolderViews[i];
                cardsHolderView.Show(i);
            }
        }

        private void PickCard(int index)
        {
            if (_isCardPicked || index >= _shownCards.Count) return;

            _isCardPicked = true;
            var pickedCard = _shownCards[index];
            TurnCardsOff(index);
            var card = _shownCards[index];
            
            cardsHolderViews[index].TurnCard(card.cardName, card.cardDescription, card.cardUiSprite);
            

            World.Default.GetRequest<CardPickRequest>().Publish(new CardPickRequest
            {
                cardType = pickedCard.cardType
            });
        }

        private void TurnCardsOff(int index)
        {
            for (var i = 0; i < cardsHolderViews.Count; i++)
            {
                if (i == index)
                {
                    cardsHolderViews[i].PlaySelected(Vector2.zero);
                }
                else
                {
                    cardsHolderViews[i].Hide();
                }
            }
        }
    }
}
