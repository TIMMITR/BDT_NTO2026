using System;
using System.Collections.Generic;
using Configs;
using Features.InputPack.Requests;
using Features.ItemsPack.Cards.Events;
using Features.ItemsPack.Cards.Requests;
using Features.RaycastPack;
using Scellecs.Morpeh;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.ItemsPack.Cards
{
    public sealed class CardsSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<RaycastComponent> _raycastStash;
        private Event<LmbStartEvent> _lmbStartEvent;
        private Event<ShowCardsEvent> _showCardsEvent;
        private Request<CardPickRequest> _request;
        private readonly CardsConfigSo _cardsConfig;


        public CardsSystem(CardsConfigSo cardsConfig)
        {
            _cardsConfig = cardsConfig;
        }

        public void OnAwake()
        {
            _filter = World.Filter.With<RaycastComponent>().Build();
            _lmbStartEvent = World.GetEvent<LmbStartEvent>();
            _raycastStash = World.GetStash<RaycastComponent>();
            _showCardsEvent  = World.GetEvent<ShowCardsEvent>();
            _request = World.GetRequest<CardPickRequest>();
            
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var _ in _lmbStartEvent.publishedChanges)
            {
                if (_filter.IsEmpty()) return;
                ref var raycastComponent = ref _raycastStash.Get(_filter.First());
                if (!raycastComponent.hasHit) continue;

                if (!raycastComponent.hitCollider.gameObject.GetComponent<CardItemView>()) continue;
                
                var availableCards = new List<CardsSo>(_cardsConfig.cards);
                var data = new List<CardsHolderData>();
                for (int i = 0; i < 3 && availableCards.Count > 0; i++)
                {
                    var randomIndex = Random.Range(0, availableCards.Count);
                    data.Add(availableCards[randomIndex].cardsHolderData);
                    availableCards.RemoveAt(randomIndex);
                }
                _showCardsEvent.NextFrame(new ShowCardsEvent
                {
                    cardsHolderDatas = data
                });
            }

            foreach (var cardPick in _request.Consume())
            {
                switch (cardPick.cardType)
                {
                    // и вот тут применять бонусы на основе cardPick.cardType
                    case CardType.CoolCard:
                        Debug.Log("Cool card");
                        break;
                    case CardType.DeadCard:
                        Debug.Log("Ded card");
                        break;
                    case CardType.HealingCard:
                        Debug.Log("Healing card");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public void Dispose() { }
    }
}