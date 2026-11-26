using System.Collections.Generic;
using Features.BattlePack;
using Features.StoryTalkPack.Data;
using UnityEngine;

namespace Features.GamePack
{
    [CreateAssetMenu(fileName = "GamePlan", menuName = "Plan/Game Plan", order = 5)]
    public class GamePlan : ScriptableObject
    {
        [SerializeField] private List<BattleSet> battleSets = new();
        
        public List<BattleSet> BattleSets => battleSets;
    }

    [System.Serializable]
    public struct BattleSet 
    {
        public BattlePlan  battlePlan;
        public StoryTalk storyTalkAfterBattle;
    }
}