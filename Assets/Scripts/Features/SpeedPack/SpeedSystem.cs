using System;
using Features.HealthPack;
using Scellecs.Morpeh;

namespace Features.SpeedPack
{
    public sealed class SpeedSystem : ISystem
    {
        public World World { get; set; }

        private Stash<SpeedComponent> _speedStash;

        private Request<SpeedRequest> _speedChangeRequest;

        private Event<SpeedEvent> _speedChangeEvent;
 
        public void OnAwake()
        {
            _speedStash = World.GetStash<SpeedComponent>();

            _speedChangeRequest = World.GetRequest<SpeedRequest>();

            _speedChangeEvent = World.GetEvent<SpeedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _speedChangeRequest.Consume())
            {
                if (!_speedStash.Has(request.entity)) continue;

                ref var speedComponent = ref _speedStash.Get(request.entity);
                var newAmount = speedComponent.currentSpeed - request.amount;
                var delta = speedComponent.currentSpeed - newAmount;

                speedComponent.currentSpeed = newAmount;

                _speedChangeEvent.NextFrame(new SpeedEvent
                {
                    entity = request.entity,
                    newAmount = newAmount,
                    delta = delta
                });
            }
        }

        public void Dispose() { }
    }
}
