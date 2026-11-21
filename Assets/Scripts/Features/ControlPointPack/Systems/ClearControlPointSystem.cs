using Features.ControlPointPack.Components;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Scellecs.Morpeh;

namespace Features.ControlPointPack.Systems
{
    public class ClearControlPointSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _controlPointFilter;
        
        private Stash<ControlPointActivatedTag> _controlPointStash;
        
        private Event<PhaseChangedEvent> _phaseChangedEvent;
            
        public void OnAwake()
        {
            _controlPointFilter = World.Filter
                .With<ControlPointActivatedTag>()
                .Build();
            
            _controlPointStash = World.GetStash<ControlPointActivatedTag>();
            
            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _phaseChangedEvent.publishedChanges)
            {
                if (request.phase != TurnPhase.Events || request.type != TurnType.End)
                    continue;
                    
                foreach (var controlPoint in  _controlPointFilter)
                    _controlPointStash.Remove(controlPoint);
            }
        }
        
        public void Dispose() { }
    }
}
