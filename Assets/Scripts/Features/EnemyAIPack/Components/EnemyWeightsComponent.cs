using System.Collections.Generic;
using Configs;
using Scellecs.Morpeh;

namespace Features.EnemyAIPack.Components
{
    public struct EnemyWeightsComponent : IComponent
    {
        public Dictionary<EnemyAIAction, float> weights;
        public Dictionary<EnemyAIAction, float> reduceValues;
        public Dictionary<EnemyAIAction, float> increaseValues;

        public EnemyWeightsComponent(EnemyConfigSo config)
        {
            weights = new Dictionary<EnemyAIAction, float>
            {
                [EnemyAIAction.Attack] = config.AttackWeight,
                [EnemyAIAction.Capture] = config.CaptureWeight,
                [EnemyAIAction.Defence] = config.DefenceWeight,
            };

            reduceValues = new Dictionary<EnemyAIAction, float>()
            {
                [EnemyAIAction.Attack] = config.AttackReduceValue,
                [EnemyAIAction.Capture] = config.CaptureReduceValue,
                [EnemyAIAction.Defence] = config.DefenceReduceValue,
            };
            
            increaseValues = new Dictionary<EnemyAIAction, float>()
            {
                [EnemyAIAction.Attack] = config.AttackIncreaseValue,
                [EnemyAIAction.Capture] = config.CaptureIncreaseValue,
                [EnemyAIAction.Defence] = config.DefenceIncreaseValue,
            };
        }
    }
}
