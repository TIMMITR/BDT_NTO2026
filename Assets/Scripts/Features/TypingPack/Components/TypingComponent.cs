using Features.TypingPack.Data;
using Scellecs.Morpeh;

namespace Features.TypingPack.Components
{
    public struct TypingComponent : IComponent
    {
        public DialogueLine line;
        public float visibleChars;
        public bool isTyping;
        public float stayTime;
        public bool active;
    }
}
