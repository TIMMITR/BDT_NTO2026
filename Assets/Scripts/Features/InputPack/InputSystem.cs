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
        

        private readonly PlayerInputActions _playerInputActions;
        
        private InputAction _lookAction;
        private InputAction _jumpAction;

        public InputSystem(PlayerInputActions playerInputActions)
        { _playerInputActions = playerInputActions; }
        
        public void OnAwake()
        {
            _filter = World.Filter.With<InputComponent>().With<PlayerTag>().Build();
            _inputStash = World.GetStash<InputComponent>();
            
            _lookAction = _playerInputActions.Gameplay.Look;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_filter.IsEmpty()) return;
            var playerEntity = _filter.First();
            
            ref var component = ref _inputStash.Get(playerEntity);
            
            component.lookVector = _lookAction.ReadValue<Vector2>();

        }

        public void Dispose() { }
    }
}