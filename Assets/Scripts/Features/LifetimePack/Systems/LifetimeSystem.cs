using System;
using Features.LifetimePack.Components;
using Features.TransformPack;
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

        public void OnAwake()
        {
            _lifetimeFilter = World.Filter.With<LifetimeComponent>().Build();
            
            _lifetimeStash = World.GetStash<LifetimeComponent>();
            _transformStash = World.GetStash<TransformComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _lifetimeFilter)
            {
                ref var ltComp = ref _lifetimeStash.Get(entity); 
                
                ltComp.remainingTime -= deltaTime;

                if (ltComp.remainingTime > 0f)
                    continue;
                
                if(_transformStash.Has(entity))
                    Object.Destroy(_transformStash.Get(entity).value.gameObject);
                
                World.RemoveEntity(entity);
            }
        }
    
        public void Dispose()
        {
            
        }
    }
}
