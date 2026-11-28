using System.Collections.Generic;
using Configs;
using Scellecs.Morpeh;

namespace Features.EnemyAIPack.Components
{
    public class EnemyTurnComponent : IComponent
    {
        public EnemyConfigSo config;
        public Dictionary<EnemyAIAction, float> weights;
        public Dictionary<EnemyAIAction, float> reduce;
    }
}

