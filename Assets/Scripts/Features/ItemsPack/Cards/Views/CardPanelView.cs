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
                cardsHolderViews[i].cardsButton.onClick.AddListener(() =>
                {
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
                        cardsHolderView.cardName.text = change.cardsHolderDatas[i].cardName;
                        cardsHolderView.image.sprite = change.cardsHolderDatas[i].cardUiSprite;
                        cardsHolderView.cardDescription.text = change.cardsHolderDatas[i].cardDescription;
                        
                        cardsHolderView.gameObject.SetActive(true);
                    }
                }
            });
        }
    }
}