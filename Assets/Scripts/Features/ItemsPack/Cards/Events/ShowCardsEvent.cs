using System.Collections.Generic;
using Scellecs.Morpeh;

namespace Features.ItemsPack.Cards.Events
{
    public struct ShowCardsEvent : IEventData
    {
        public List<CardsHolderData>  cardsHolderDatas;
    }
}