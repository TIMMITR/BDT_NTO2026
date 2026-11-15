using Configs;
using Features.CameraPack.Components;
using Features.TargetPack.Views;
using Features.TransformPack;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CameraPack.Systems
{
    public class CameraSpawnSystem : IInitializer
    {
        public World World { get; set; }

        private readonly PrefabSettingsSo _prefabSettings;
        private Filter _cameraFilter;
        private Filter _cameraTargetFilter;
        public CameraSpawnSystem(PrefabSettingsSo prefabSettings)
        {
            _prefabSettings = prefabSettings;
        }
        
        public void OnAwake()
        {

            _cameraFilter = World.Filter.With<CameraComponent>().Build();
            _cameraTargetFilter  = World.Filter.With<CameraTargetTag>().Build();
            
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

        public void Dispose()
        {
            foreach (var entity in _cameraTargetFilter)
            {
                World.RemoveEntity(entity);
            }
            foreach (var entity in _cameraFilter)
            {
                World.RemoveEntity(entity);
            }
        }
    }
}