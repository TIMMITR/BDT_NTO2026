using System;
using System.Collections.Generic;
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

        private void Awake()
        {
            for (var i = 0; i < cardsHolderViews.Count; i++)
            {
                var index = i;
                cardsHolderViews[i].gameObject.SetActive(false);
                cardsHolderViews[i].cardsButton.onClick.AddListener(() =>
                {
                    cardsHolderViews[index].TurnCard
                        (_shownCards[index].cardName,
                        _shownCards[index].cardDescription,
                        _shownCards[index].cardUiSprite);
                    TurnCardsOff(index);
                    World.Default.GetRequest<CardPickRequest>().Publish(new CardPickRequest
                    {
                        cardType = _shownCards[index].cardType
                    });
                });
            }
            
            World.Default.GetEvent<ShowCardsEvent>().Subscribe(changes =>
            {
                foreach (var change in changes)
                {
                    _shownCards = change.cardsHolderDatas;
                    for (var i = 0; i < cardsHolderViews.Count &&  i < change.cardsHolderDatas.Count; i++)
                    {
                        var cardsHolderView = cardsHolderViews[i];
                        
                        cardsHolderView.gameObject.SetActive(true);
                    }
                }
            });
        }

        public void TurnCardsOff(int index)
        {
            for (var i = 0; i < cardsHolderViews.Count && i < _shownCards.Count; i++)
            {
                if (i == index) continue;
                cardsHolderViews[i].gameObject.SetActive(false);
            }
            // todo корутиной  или primeTween отключать карту index
        }
        
    }
}