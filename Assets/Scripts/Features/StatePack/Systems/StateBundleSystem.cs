using Features.SpeedPack.Requests;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Scellecs.Morpeh;

namespace Features.StatePack.Systems
{
    public class StateBundleSystem : ISystem
    {
        public World World { get; set; }
        
        private Request<ResetSpeedRequest> _resetSpeedRequest;
        
        private Event<PhaseChangedEvent> _phaseChangedEvent;

        public void OnAwake()
        {
            _resetSpeedRequest = World.GetRequest<ResetSpeedRequest>();
            
            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _phaseChangedEvent.publishedChanges)
            {
                switch (evt.phase, evt.type)
                {
                    case (TurnPhase.Player, TurnType.End):
                        _resetSpeedRequest.Publish(new ResetSpeedRequest());
                        break;
                    
                    case (TurnPhase.Enemy, TurnType.End):
                        _resetSpeedRequest.Publish(new ResetSpeedRequest());
                        break;
                }
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
