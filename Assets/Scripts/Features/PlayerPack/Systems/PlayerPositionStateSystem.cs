using Features.PlayerPack.Events;
using Features.PlayerPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.PlayerPack.Systems
{
    public class PlayerPositionStateSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<PlayerPositionComponent> _stash;
        
        private Request<PlayerStateChangeRequest> _request;
        private Event<PlayerStateChangeEvent> _event;

        public void OnAwake()
        {
            _filter = World.Filter.With<PlayerPositionComponent>().Build();
            _stash = World.GetStash<PlayerPositionComponent>();
            _request = World.GetRequest<PlayerStateChangeRequest>();
            _event = World.GetEvent<PlayerStateChangeEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_filter.IsEmpty()) return;
            
            foreach (var _ in _request.Consume())
            {
                ref var playerPositionComponent = ref _stash.Get(_filter.First());
                playerPositionComponent.positionState = !playerPositionComponent.positionState;
                
                _event.NextFrame(new PlayerStateChangeEvent
                {
                    newPositionState = playerPositionComponent.positionState
                });
            }
        }

        public void Dispose() { }
    }
}