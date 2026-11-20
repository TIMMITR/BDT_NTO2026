using Features.StatePack.Components;
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
        }

        public void OnUpdate(float deltaTime)
        {
            var turnPhaseEntity = _turnPhaseFilter.First();
            ref var turnPhaseComp = ref _turnPhaseStash.Get(turnPhaseEntity);
            
            foreach (var _ in _startPlayerTurnRequest.Consume())
            {
                Debug.Log("Player phase started");
                
                turnPhaseComp.phase = TurnPhase.Player;
            }
            
            foreach (var _ in _endPlayerTurnRequest.Consume())
            {
                Debug.Log("Player phase ender");
                
                turnPhaseComp.phase = TurnPhase.None;
            }
            
            foreach (var _ in _startEnemyTurnRequest.Consume())
            {
                Debug.Log("Enemy phase started");
                
                turnPhaseComp.phase = TurnPhase.Player;
            }
            
            foreach (var _ in _endEnemyTurnRequest.Consume())
            {
                Debug.Log("Enemy phase ender");
                
                turnPhaseComp.phase = TurnPhase.None;
            }
            
            foreach (var _ in _startEventsPhaseRequest.Consume())
            {
                Debug.Log("Events phase started");
                
                turnPhaseComp.phase = TurnPhase.Player;
            }
            
            foreach (var _ in _endEventsPhaseRequest.Consume())
            {
                Debug.Log("Events phase ender");
                
                turnPhaseComp.phase = TurnPhase.None;
            }
        }
        
        public void Dispose() { }
    }
}
