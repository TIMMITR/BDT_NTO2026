using System;
using Features.LifetimePack.Components;
using Features.TransformPack;
using Global;
using Scellecs.Morpeh;
using Object = UnityEngine.Object;

namespace Features.LifetimePack.Systems
{
    public class LifetimeSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _lifetimeFilter;
        
        private Stash<LifetimeComponent> _lifetimeStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<DestroyTag> _destroyTag;

        public void OnAwake()
        {
            _lifetimeFilter = World.Filter.With<LifetimeComponent>().Build();
            
            _lifetimeStash = World.GetStash<LifetimeComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _destroyTag = World.GetStash<DestroyTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _lifetimeFilter)
            {
                ref var ltComp = ref _lifetimeStash.Get(entity); 
                
                ltComp.remainingTime -= deltaTime;

                if (ltComp.remainingTime > 0f)
                    continue;
                
                _destroyTag.Add(entity);
            }
        }
    
        public void Dispose()
        {
            
        }
    }
}
