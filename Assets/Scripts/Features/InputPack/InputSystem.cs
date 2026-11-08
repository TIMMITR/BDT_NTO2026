using Features.InputPack.Requests;
using Global;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.InputPack
{
    public sealed class InputSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<InputComponent> _inputStash;
        
        private Request<JumpStartRequest> _jumpStartRequest;
        private Request<JumpEndRequest> _jumpEndRequest;

        private readonly PlayerInputActions _playerInputActions;
        
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;

        public InputSystem(PlayerInputActions playerInputActions)
        { _playerInputActions = playerInputActions; }
        
        public void OnAwake()
        {
            _filter = World.Filter.With<InputComponent>().With<PlayerTag>().Build();
            _inputStash = World.GetStash<InputComponent>();
            
            _jumpStartRequest = World.GetRequest<JumpStartRequest>();
            _jumpEndRequest = World.GetRequest<JumpEndRequest>();
            
            _moveAction = _playerInputActions.Gameplay.Move;
            _lookAction = _playerInputActions.Gameplay.Look;
            _jumpAction = _playerInputActions.Gameplay.Jump;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_filter.IsEmpty()) return;
            var playerEntity = _filter.First();
            
            ref var component = ref _inputStash.Get(playerEntity);
            
            component.moveVector = _moveAction.ReadValue<Vector2>();
            component.lookVector = _lookAction.ReadValue<Vector2>();

            if (_jumpAction.WasPressedThisFrame())
                _jumpStartRequest.Publish(new JumpStartRequest { entity = playerEntity });

            if (_jumpAction.WasReleasedThisFrame())
                _jumpEndRequest.Publish(new JumpEndRequest { entity = playerEntity });
        }

        public void Dispose() { }
    }
}