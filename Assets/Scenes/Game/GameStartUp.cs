using Configs;
using Features.CameraPack.Systems;
using Features.CharacterPack.Systems;
using Features.DamagePack;
using Features.HealthPack;
using Features.HexGridPack;
using Features.InputPack;
using Features.LevelPack.Systems;
using Features.ManaPack;
using Features.PlayerPack.Systems;
using Features.RaycastPack;
using Features.ScenePack;
using Features.TargetPack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Scenes.Game
{
    public class GameStartUp : MonoBehaviour
    {
        [SerializeField] private GlobalContext globalContext;
        [SerializeField] private GameContext gameContext;

        private World _world;
        private SystemsGroup _initGroup;
        private SystemsGroup _updateGroup;
        private SystemsGroup _fixedGroup;
        private PlayerInputActions _playerInputActions;

        private void Awake()
        {
            _world = World.Default;
            
            _playerInputActions =  new PlayerInputActions();
            _playerInputActions.Enable();
            
            _initGroup = _world.CreateSystemsGroup();
            _initGroup.AddInitializer(new PlayerSpawnSystem(
                globalContext.prefabSettings,
                globalContext.worldSettings,
                gameContext.playerSpawnPoint));
            _initGroup.AddInitializer(new CameraSpawnSystem(
                globalContext.prefabSettings));
            _initGroup.AddInitializer(new HexGridSystem(
                gameContext.boardGameObject,
                gameContext.levelConfig)); 
            _initGroup.AddInitializer(new CharactersInitSystem(
                gameContext.characters));
            _initGroup.Initialize();
            
            _updateGroup = _world.CreateSystemsGroup();
            _updateGroup.Initialize();
            _updateGroup.AddSystem(new InputSystem(_playerInputActions));
            _updateGroup.AddSystem(new TargetSystem());
            _updateGroup.AddSystem(new RaycastSystem());
            _updateGroup.AddSystem(new ChooseCellSystem());
            
            _updateGroup.AddSystem(new ManaSystem());
            _updateGroup.AddSystem(new DamageSystem());
            _updateGroup.AddSystem(new HealthSystem());
            
            _updateGroup.AddSystem(new SceneSystem(globalContext.transitionSettings));
                
            _updateGroup.Initialize();
            
            _world.AddSystemsGroup(order: 0, _updateGroup);

            _fixedGroup = _world.CreateSystemsGroup();
            _fixedGroup.Initialize();
        }

        private void FixedUpdate()
        {
            _fixedGroup?.Update(Time.fixedDeltaTime);
        }

        private void OnDestroy()
        {
            if (_updateGroup != null)
            {
                _world?.RemoveSystemsGroup(_updateGroup);
                _updateGroup.Dispose();
                _updateGroup = null;
            }

            if (_fixedGroup != null)
            {
                _fixedGroup.Dispose();
                _fixedGroup = null;
            }
        }
    }
}