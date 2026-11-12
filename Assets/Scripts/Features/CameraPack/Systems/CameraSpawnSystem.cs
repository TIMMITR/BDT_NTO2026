using Configs;
using Features.CameraPack.Components;
using Features.TargetPack.Views;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CameraPack.Systems
{
    public class CameraSpawnSystem : IInitializer
    {
        public World World { get; set; }

        private readonly PrefabSettingsSo _prefabSettings;
        
        public CameraSpawnSystem(PrefabSettingsSo prefabSettings)
        {
            _prefabSettings = prefabSettings;
        }
        
        public void OnAwake()
        {
            var cameraGameObject = Object.Instantiate(_prefabSettings.Get(PrefabType.Camera));
            
            var camera = cameraGameObject.GetComponentInChildren<Camera>();
            var target = cameraGameObject.GetComponentInChildren<CameraTargetView>();
            
            var cameraEntity = World.CreateEntity();
            ref var cameraComponent = ref World.GetStash<CameraComponent>().Add(cameraEntity);
            cameraComponent.value = camera;
            World.GetStash<CameraTag>().Add(cameraEntity);
            
            var cameraTargetEntity = World.CreateEntity();
            ref var transformComponent = ref World.GetStash<TransformComponent>().Add(cameraTargetEntity);
            transformComponent.value = target.gameObject.transform;
            
            World.GetStash<CameraTargetTag>().Add(cameraTargetEntity);
            
        }
        
        public void Dispose() { }
    }
}