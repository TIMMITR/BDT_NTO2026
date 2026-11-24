using Features.TransformPack;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.DeathPack.Systems
{
    public class EntityDestroySystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _destroyFilter;
        
        private Stash<TransformComponent> _transformStash;

        public void OnAwake()
        {
            _destroyFilter = World.Filter.
                With<DestroyTag>().
                Build();
            
            _transformStash = World.GetStash<TransformComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var unit in _destroyFilter)
            {
                Debug.Log($"Entity: {unit} has been destroyed");

                if (_transformStash.Has(unit)) // не вынесено в фильтр, т.к. некоторые сущности не содержат этот компонент
                {
                    ref var tr = ref _transformStash.Get(unit);
                    Object.Destroy(tr.value.gameObject);
                }
                
                World.RemoveEntity(unit);
            }
        }
    
        public void Dispose() {}
    }
}
