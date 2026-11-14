using System.Collections.Generic;
using Features.BattlePack;
using UnityEngine;

namespace Features.GamePack
{
    [CreateAssetMenu(fileName = "GamePlan", menuName = "Plan/Game Plan", order = 5)]
    public class GamePlan : ScriptableObject
    {
        [SerializeField] private List<BattlePlan> battlePlans = new();
        
        public List<BattlePlan> BattlePlans => battlePlans;
    }
}