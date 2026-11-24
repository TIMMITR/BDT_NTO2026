using Features.HealthPack;
using Global;
using Scellecs.Morpeh;

namespace Features.DeathPack.Systems
{
    public class DeathTagsApplySystem : ISystem
    {
        public World World { get; set; }
        
        private Stash<DeadTag> _deadStash;
        private Stash<DestroyTag> _destroyStash;
        
        private Event<HealthChangedEvent> _healthChangedEvent;
        
        public void OnAwake()
        {
            _deadStash = World.GetStash<DeadTag>();
            _destroyStash = World.GetStash<DestroyTag>();
            
            _healthChangedEvent = World.GetEvent<HealthChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _healthChangedEvent.publishedChanges)
            {
                var unit = evt.entity;
                
                if(evt.newAmount > 0 || World.IsDisposed(unit) || _destroyStash.Has(unit))
                    continue;
                
                _deadStash.Add(unit);
                _destroyStash.Add(unit);
            }
        }
    
        public void Dispose() {}
    }
}
