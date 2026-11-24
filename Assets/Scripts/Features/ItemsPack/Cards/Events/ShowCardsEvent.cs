using System.Collections.Generic;
using Features.ItemsPack.Cards.Data;
using Scellecs.Morpeh;

namespace Features.ItemsPack.Cards.Events
{
    public struct ShowCardsEvent : IEventData
    {
        public List<CardsHolderData>  cardsHolderDatas;
    }
}