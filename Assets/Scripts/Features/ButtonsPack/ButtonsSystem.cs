using System;
using Features.InputPack.Requests;
using Features.RaycastPack;
using Features.ScenePack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.ButtonsPack
{
    public sealed class ButtonsSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<RaycastComponent> _raycastStash;
        private Request<SceneRequest> _sceneRequest;
        
        private Request<LmbStartRequest> _lmbStartRequest;
        
        private readonly string _gameSceneName;
        public ButtonsSystem(string gameSceneName)
        {
            _gameSceneName = gameSceneName;
        }
        public void OnAwake()
        {
            _filter = World.Filter.With<RaycastComponent>().Build();
            _raycastStash = World.GetStash<RaycastComponent>();
            _sceneRequest = World.GetRequest<SceneRequest>();
            _lmbStartRequest = World.GetRequest<LmbStartRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var _ in _lmbStartRequest.Consume())
            {
                ref var raycastComponent = ref _raycastStash.Get(_filter.First());
                if (!raycastComponent.hasHit) return;
                var buttonView = raycastComponent.hitCollider.gameObject.GetComponent<ButtonView>();
                if (buttonView != null)
                {
                    switch (buttonView.type)
                    {
                        case ButtonType.Start:
                            _sceneRequest.Publish(new SceneRequest
                            {
                                sceneName = _gameSceneName
                            });
                            break;
                        case ButtonType.Exit:
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                            break;
                        case ButtonType.Setting:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
        }

        public void Dispose() { }
    }
}