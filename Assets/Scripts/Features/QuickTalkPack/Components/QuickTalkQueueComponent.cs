using System.Collections.Generic;
using Features.QuickTalkPack.Data;
using Scellecs.Morpeh;

namespace Features.QuickTalkPack.Components
{
    public class QuickTalkCandidate
    {
        public QuickTalkSet set;
        public float ttlLeft;
    }

    public struct QuickTalkQueueComponent : IComponent
    {
        public List<QuickTalkCandidate> candidates;
        public float globalCooldownLeft;
        public Dictionary<QuickTalkSet, float> cooldowns;
        public HashSet<QuickTalkSet> saidThisBattle;
        public Dictionary<QuickTalkSet, int> lastLineIndexes; 
    }
}
