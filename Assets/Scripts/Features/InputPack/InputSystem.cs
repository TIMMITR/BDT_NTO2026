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
        
        private Request<EscRequest> _escRequest;
        
        private Request<LmbStartRequest> _lmbStartRequest;
        private Request<LmbEndRequest> _lmbEndRequest;
        
        private Request<RmbStartRequest> _rmbStartRequest;
        private Request<RmbEndRequest> _rmbEndRequest;
        
        private InputAction _lookAction;
        private InputAction _escAction;
        
        private InputAction _lmbAction;
        private InputAction _rmbAction;
        
        private readonly PlayerInputActions _playerInputActions;

        public InputSystem(PlayerInputActions playerInputActions)
        { _playerInputActions = playerInputActions; }
        
        public void OnAwake()
        {
            _filter = World.Filter.With<InputComponent>().With<PlayerTag>().Build();
            _inputStash = World.GetStash<InputComponent>();
            
            _escRequest = World.GetRequest<EscRequest>();
            
            _lmbStartRequest = World.GetRequest<LmbStartRequest>();
            _lmbEndRequest = World.GetRequest<LmbEndRequest>();
            
            _rmbStartRequest = World.GetRequest<RmbStartRequest>();
            _rmbEndRequest = World.GetRequest<RmbEndRequest>();
            
            _lookAction = _playerInputActions.Gameplay.Look;
            _escAction = _playerInputActions.Gameplay.Esc;
            
            _lmbAction = _playerInputActions.Gameplay.LMB;
            _rmbAction = _playerInputActions.Gameplay.RMB;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_filter.IsEmpty()) return;
            var playerEntity = _filter.First();
            
            ref var component = ref _inputStash.Get(playerEntity);
            
            component.lookVector = _lookAction.ReadValue<Vector2>();
            
            if (_escAction.WasPerformedThisFrame())
                _escRequest.Publish(new EscRequest{ entity = playerEntity });
            
            if (_lmbAction.WasPressedThisFrame())
                _lmbStartRequest.Publish(new LmbStartRequest{ entity = playerEntity });

            if (_lmbAction.WasReleasedThisFrame())
                _lmbEndRequest.Publish(new LmbEndRequest{ entity = playerEntity });

            if (_rmbAction.WasPressedThisFrame())
                _rmbStartRequest.Publish(new RmbStartRequest{ entity = playerEntity });

            if (_rmbAction.WasReleasedThisFrame())
                _rmbEndRequest.Publish(new RmbEndRequest{ entity = playerEntity });
            
        }

        public void Dispose() { }
    }
}