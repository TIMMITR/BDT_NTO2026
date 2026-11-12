using Configs;
using Features.InputPack;
using Features.ManaPack;
using Features.TargetPack.Views;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.PlayerPack.Systems
{
    public class PlayerSpawnSystem : IInitializer
    {
        public World World { get; set; }
        
        private readonly Transform _playerStartPoint;
        private readonly WorldSettingsSo _worldSettings;
        private readonly PrefabSettingsSo _prefabSettings;
        
        public PlayerSpawnSystem(
            PrefabSettingsSo prefabSettings,
            WorldSettingsSo worldSettings,
            Transform playerStartPoint)
        {
            _prefabSettings = prefabSettings;
            _worldSettings = worldSettings;
            _playerStartPoint = playerStartPoint;
        }
        
        public void OnAwake()
        {
            var playerGameObject = Object.Instantiate(_prefabSettings.Get(PrefabType.Player));
            playerGameObject.transform.position = _playerStartPoint.position;
            playerGameObject.transform.rotation = _playerStartPoint.rotation;
            
            var target = playerGameObject.GetComponentInChildren<PlayerTargetView>();
            // сущность игрока
            var playerEntity = World.CreateEntity();
            
            World.GetStash<InputComponent>().Add(playerEntity);
            
            ref var manaComponent = ref World.GetStash<ManaComponent>().Add(playerEntity);
            manaComponent.maxMana = _worldSettings.playerMaxMana;
            
            ref var playerTransformComp = ref World.GetStash<TransformComponent>().Add(playerEntity);
            playerTransformComp.value = playerGameObject.transform;
            
            World.GetStash<PlayerTag>().Add(playerEntity);
            
            // сущность цели в игроке 
            var targetEntity = World.CreateEntity();
            
            ref var targetTransformComp= ref World.GetStash<TransformComponent>().Add(targetEntity);
            targetTransformComp.value = target.transform;
            
            World.GetStash<PlayerTargetTag>().Add(targetEntity);
        }
        
        public void Dispose() { }
    }
}
