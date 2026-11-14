using Features.TypingPack.Data;
using Scellecs.Morpeh;

namespace Features.TypingPack.Requests
{
    public struct TypingStartRequest : IRequestData
    {
        public DialogueLine line;
    }
}
