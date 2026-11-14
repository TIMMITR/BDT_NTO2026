using System.Collections.Generic;
using Features.TypingPack.Data;
using UnityEngine;

namespace Features.StoryTalkPack.Data
{
    [CreateAssetMenu(fileName = "StoryTalk", menuName = "Dialogue/Story Talk", order = 10)]
    public class StoryTalk : ScriptableObject
    {
        
        // сдесь еще что-то можно хранить
        [SerializeField] private List<DialogueLine> lines = new();

        public IReadOnlyList<DialogueLine> Lines => lines;
    }
}
