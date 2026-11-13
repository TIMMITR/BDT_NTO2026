using Plugins.EasyTransitions.Scripts;
using Scellecs.Morpeh;

namespace Features.ScenePack
{
    public struct SceneRequest : IRequestData
    {
        public TransitionSettings transition;
        public string sceneName;
    }
}