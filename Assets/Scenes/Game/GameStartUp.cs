using Features.BattlePack;
using Features.CameraPack.Systems;
using Features.CellSelectionPack.Systems;
using Features.CellTrackPack.Systems;
using Features.ControlPointPack.Systems;
using Features.DamagePack;
using Features.DeathPack.Systems;
using Features.EnemyAIPack.System;
using Features.FightPack.Systems;
using Features.GridPack;
using Features.StoryTalkPack.Systems;
using Features.TypingPack.Systems;
using Features.HealthPack;
using Features.InputPack;
using Features.ItemsPack.Cards;
using Features.KnockbackPack.Systems;
using Features.LifetimePack.Systems;
using Features.ManaPack;
using Features.PlayerPack.Systems;
using Features.QuickTalkPack.Systems;
using Features.RaycastPack;
using Features.ScenePack;
using Features.SpeedPack.Systems;
using Features.SpellPack.Systems;
using Features.StatePack.Systems;
using Features.TargetPack;
using Features.UnitPack.Systems;
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
                globalContext.prefabSettings,
                gameContext.playerSpawnPoint));
            _initGroup.AddInitializer(new GridSystem(
                globalContext.worldSettings)); 
            _initGroup.Initialize();
            
            _updateGroup = _world.CreateSystemsGroup();
            _updateGroup.Initialize();
            _updateGroup.AddSystem(new InputSystem(_playerInputActions));
            _updateGroup.AddSystem(new TargetSystem());
            _updateGroup.AddSystem(new RaycastSystem());
            
            _updateGroup.AddSystem(new StateChangeSystem());
            _updateGroup.AddSystem(new StateBundleSystem());
            
            _updateGroup.AddSystem(new ChooseCellSystem());
            _updateGroup.AddSystem(new CameraTargetMoveSystem(
                gameContext.cameraCorridorStart,
                gameContext.cameraCorridorEnd));
            _updateGroup.AddSystem(new TrackChosenUnitSystem());
            _updateGroup.AddSystem(new BattleSystem(gameContext.gamePlan));

            _updateGroup.AddSystem(new SpawnControlPointSystem());
            _updateGroup.AddSystem(new CaptureControlPointSystem());
            _updateGroup.AddSystem(new ActivateControlPointSystem());
            _updateGroup.AddSystem(new ClearControlPointSystem());

            _updateGroup.AddSystem(new CardsSystem(globalContext.cardsConfig));
            
            _updateGroup.AddSystem(new SelectUnitSystem());
            _updateGroup.AddSystem(new UnitSpawnSystem());
            _updateGroup.AddSystem(new MoveCommandSystem());
            _updateGroup.AddSystem(new PathPreviewSystem());
            _updateGroup.AddSystem(new DelayAttackSystem());
            _updateGroup.AddSystem(new ProjectileSystem());
            _updateGroup.AddSystem(new AttackSystem());
            _updateGroup.AddSystem(new ColorCellSystem(gameContext.highlightCellPool, globalContext.worldSettings.colorData));
            _updateGroup.AddSystem(new UnitAnimationSystem());
            _updateGroup.AddSystem(new ChangeAnimationDataSystem(gameContext.moveDataList));
            _updateGroup.AddSystem(new PlayerPositionStateSystem());

            _updateGroup.AddSystem(new KnockbackSystem());
            _updateGroup.AddSystem(new LifetimeSystem());
            
            _updateGroup.AddSystem(new ManaSystem());
            _updateGroup.AddSystem(new DamageSystem());
            _updateGroup.AddSystem(new HealthSystem());
            _updateGroup.AddSystem(new SpeedSystem());
            _updateGroup.AddSystem(new ResetSpeedSystem());

            _updateGroup.AddSystem(new EnemyAISystem());

            _updateGroup.AddSystem(new SpellChangeSystem());

            _updateGroup.AddSystem(new DeathSystem());
            
            _updateGroup.AddSystem(new StoryTalkPlayerSystem(gameContext.introTalk));
            
            _updateGroup.AddSystem(new QuickTalkTriggerSystem(gameContext.gamePlan, globalContext.worldSettings));
            _updateGroup.AddSystem(new QuickTalkQueueSystem(globalContext.worldSettings));
            
            _updateGroup.AddSystem(new TypingSystem(globalContext.worldSettings));
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