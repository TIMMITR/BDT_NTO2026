using Configs;
using Plugins.EasyTransitions.Scripts;
using Scellecs.Morpeh;

namespace Features.ScenePack
{
    public sealed class SceneSystem : ISystem
    {
        public World World { get; set; }

        private Request<SceneRequest> _sceneRequest;
        private readonly TransitionsSettingsSo _transitionSettings;

        public SceneSystem(TransitionsSettingsSo transitionSettings)
        {
            _transitionSettings = transitionSettings;
        }

        public void OnAwake()
        {
            _sceneRequest = World.GetRequest<SceneRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _sceneRequest.Consume())
            {
                var transition = request.transition != null
                    ? request.transition
                    : _transitionSettings.Get(TransitionType.Fade);
                
                TransitionManager.Instance()
                    .Transition(request.sceneName, transition);
                break;
            }
        }

        public void Dispose() { }
    }
}