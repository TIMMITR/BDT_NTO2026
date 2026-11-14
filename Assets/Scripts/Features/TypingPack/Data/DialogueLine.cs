using System;
using UnityEngine;

namespace Features.TypingPack.Data
{
    [Serializable]
    public struct DialogueLine
    {
        public Speaker speaker;
        [TextArea] public string text;
        public string emotion;
        public AudioClip voice;
    }
}
