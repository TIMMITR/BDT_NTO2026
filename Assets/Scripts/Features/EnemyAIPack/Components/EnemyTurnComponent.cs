using System.Collections.Generic;
using Configs;
using Scellecs.Morpeh;

namespace Features.EnemyAIPack.Components
{
    public class EnemyTurnComponent : IComponent
    {
        public EnemyConfigSo config;
        public Dictionary<AIAction, float> weights;
        public Dictionary<AIAction, float> reduce;
    }
}

