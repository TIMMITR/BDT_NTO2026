using Features.ItemsPack.Cards.Data;
using Scellecs.Morpeh;

namespace Features.ItemsPack.Cards.Requests
{
    public struct CardPickRequest : IRequestData
    {
        public CardType cardType;
    }
}