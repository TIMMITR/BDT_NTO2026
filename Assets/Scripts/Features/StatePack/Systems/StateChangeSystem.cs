using Features.StatePack.Components;
using Features.StatePack.Events;
using Features.StatePack.Requests;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.StatePack.Systems
{
    public class StateChangeSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _turnPhaseFilter;
        
        private Stash<TurnPhaseComponent> _turnPhaseStash;
        
        private Request<StartPlayerTurnRequest> _startPlayerTurnRequest;
        private Request<EndPlayerTurnRequest> _endPlayerTurnRequest;
        private Request<StartEnemyTurnRequest> _startEnemyTurnRequest;
        private Request<EndEnemyTurnRequest> _endEnemyTurnRequest;
        private Request<StartEventsPhaseRequest> _startEventsPhaseRequest;
        private Request<EndEventsPhaseRequest> _endEventsPhaseRequest;
        
        private Event<PhaseChangedEvent> _phaseChangedEvent;

        public void OnAwake()
        {
            _turnPhaseFilter = World.Filter.With<TurnPhaseComponent>().Build();
            
            _turnPhaseStash = World.GetStash<TurnPhaseComponent>();
            
            _startPlayerTurnRequest = World.GetRequest<StartPlayerTurnRequest>();
            _endPlayerTurnRequest = World.GetRequest<EndPlayerTurnRequest>();

            _startEnemyTurnRequest = World.GetRequest<StartEnemyTurnRequest>();
            _endEnemyTurnRequest = World.GetRequest<EndEnemyTurnRequest>();
            
            _startEventsPhaseRequest = World.GetRequest<StartEventsPhaseRequest>();
            _endEventsPhaseRequest = World.GetRequest<EndEventsPhaseRequest>();

            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
            
            _turnPhaseStash.Add(World.CreateEntity());
        }

        public void OnUpdate(float deltaTime)
        {
            var turnPhaseEntity = _turnPhaseFilter.First();
            ref var turnPhaseComp = ref _turnPhaseStash.Get(turnPhaseEntity);
            
            foreach (var _ in _startPlayerTurnRequest.Consume())
            {
                Debug.Log("Player phase started");
                
                turnPhaseComp.phase = TurnPhase.Player;
                _phaseChangedEvent.NextFrame(new PhaseChangedEvent(TurnPhase.Player, TurnType.Start));
            }
            
            foreach (var _ in _endPlayerTurnRequest.Consume())
            {
                if (turnPhaseComp.phase != TurnPhase.Player)
                    continue;
                
                Debug.Log("Player phase ended");
                
                turnPhaseComp.phase = TurnPhase.None;
                _phaseChangedEvent.NextFrame(new PhaseChangedEvent(TurnPhase.Player, TurnType.End));
            }
            
            foreach (var _ in _startEnemyTurnRequest.Consume())
            {
                Debug.Log("Enemy phase started");
                
                turnPhaseComp.phase = TurnPhase.Enemy;
                _phaseChangedEvent.NextFrame(new PhaseChangedEvent(TurnPhase.Enemy, TurnType.Start));
            }
            
            foreach (var _ in _endEnemyTurnRequest.Consume())
            {
                Debug.Log("Enemy phase ended");
                
                turnPhaseComp.phase = TurnPhase.None;
                _phaseChangedEvent.NextFrame(new PhaseChangedEvent(TurnPhase.Enemy, TurnType.End));
            }
            
            foreach (var _ in _startEventsPhaseRequest.Consume())
            {
                Debug.Log("Events phase started");
                
                turnPhaseComp.phase = TurnPhase.Events;
                _phaseChangedEvent.NextFrame(new PhaseChangedEvent(TurnPhase.Events, TurnType.Start));
            }
            
            foreach (var _ in _endEventsPhaseRequest.Consume())
            {
                Debug.Log("Events phase ended");
                
                turnPhaseComp.phase = TurnPhase.None;
                _phaseChangedEvent.NextFrame(new PhaseChangedEvent(TurnPhase.Events, TurnType.End));
            }
        }
        
        public void Dispose() { }
    }
}
