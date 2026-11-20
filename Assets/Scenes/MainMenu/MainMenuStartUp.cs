using Features.ButtonsPack;
using Features.CameraPack.Systems;
using Features.InputPack;
using Features.PlayerPack.Systems;
using Features.RaycastPack;
using Features.ScenePack;
using Features.SimpleAnimationPack;
using Features.TargetPack;
using Features.TransformPack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Scenes.MainMenu
{
    public class MainMenuStartUp : MonoBehaviour
    {
        [SerializeField] private GlobalContext globalContext;
        [SerializeField] private MainMenuContext mainMenuContext;

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
                mainMenuContext.playerSpawnPoint));
            _initGroup.AddInitializer(new CameraSpawnSystem(
                globalContext.prefabSettings,
                mainMenuContext.playerSpawnPoint));
            _initGroup.AddInitializer(new TransformCleanUpSystem());
            _initGroup.Initialize();
            
            _updateGroup = _world.CreateSystemsGroup();
            _updateGroup.AddSystem(new InputSystem(_playerInputActions));
            _updateGroup.AddSystem(new TargetSystem());
            _updateGroup.AddSystem(new RaycastSystem());
            // todo движение камеры на сцене как нибудь
            _updateGroup.AddSystem(new SimpleAnimationSystem(globalContext.worldSettings));
            
            _updateGroup.AddSystem(new ButtonsSystem(mainMenuContext.gameSceneName));
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
            _playerInputActions?.Disable();
            _playerInputActions?.Dispose();

            if (_initGroup != null)
            {
                _initGroup.Dispose();
                _initGroup = null;
            }
            
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