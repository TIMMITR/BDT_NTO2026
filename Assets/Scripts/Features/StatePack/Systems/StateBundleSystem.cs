using Features.SpeedPack.Requests;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Features.StatePack.Requests;
using Scellecs.Morpeh;

namespace Features.StatePack.Systems
{
    public class StateBundleSystem : ISystem
    {
        public World World { get; set; }
        
        private Request<ResetSpeedRequest> _resetSpeedRequest;
        
        private Request<StartEnemyTurnRequest> _startEnemyTurnRequest;
        private Request<StartEventsPhaseRequest> _startEventsPhaseRequest;
        private Request<EndEventsPhaseRequest> _endEventsPhaseRequest;
        private Request<StartPlayerTurnRequest> _startPlayerTurnRequest;
        
        private Event<PhaseChangedEvent> _phaseChangedEvent;

        public void OnAwake()
        {
            _resetSpeedRequest = World.GetRequest<ResetSpeedRequest>();

            _startEnemyTurnRequest = World.GetRequest<StartEnemyTurnRequest>();
            _startEventsPhaseRequest = World.GetRequest<StartEventsPhaseRequest>();
            _endEventsPhaseRequest = World.GetRequest<EndEventsPhaseRequest>();
            _startPlayerTurnRequest = World.GetRequest<StartPlayerTurnRequest>();
            
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
                        _startEnemyTurnRequest.Publish(new StartEnemyTurnRequest());
                        break;
                    
                    case (TurnPhase.Enemy, TurnType.End):
                        _resetSpeedRequest.Publish(new ResetSpeedRequest());
                        _startEventsPhaseRequest.Publish(new StartEventsPhaseRequest());
                        break;
                    
                }
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
