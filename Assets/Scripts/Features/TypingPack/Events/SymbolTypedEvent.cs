using Features.TypingPack.Data;
using Scellecs.Morpeh;

namespace Features.TypingPack.Events
{
    public struct SymbolTypedEvent : IEventData
    {
        public Speaker speaker;
    }
}