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

        private Filter _cameraFilter;
        private Filter _cameraTargetFilter;
        private readonly PrefabSettingsSo _prefabSettings;
        private readonly Transform _playerSpawnPoint;
        public CameraSpawnSystem(PrefabSettingsSo prefabSettings, Transform playerSpawnPoint)
        {
            _prefabSettings = prefabSettings;
            _playerSpawnPoint =  playerSpawnPoint;
        }
        
        public void OnAwake()
        {

            _cameraFilter = World.Filter.With<CameraComponent>().Build();
            _cameraTargetFilter  = World.Filter.With<CameraTargetTag>().Build();
            
            var cameraGameObject = Object.Instantiate(_prefabSettings.Get(PrefabType.Camera), _playerSpawnPoint.position, _playerSpawnPoint.rotation);
            
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