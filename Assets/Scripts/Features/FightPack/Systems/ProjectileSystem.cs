using System;
using Features.FightPack.Components;
using Features.FightPack.Requests;
using Features.GridPack.Components;
using Features.TransformPack;
using Scellecs.Morpeh;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Features.FightPack.Systems
{
    public class ProjectileSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        private Filter _projectileFilter;
        
        private Stash<GridComponent> _gridStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<ProjectileComponent> _projectileStash;
        
        private Request<ProjectileCastRequest> _projectileRequest;
        private Request<AttackOrderRequest> _attackOrderRequest;
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            _projectileFilter = World.Filter.With<ProjectileComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _projectileStash = World.GetStash<ProjectileComponent>();
            
            _projectileRequest = World.GetRequest<ProjectileCastRequest>();
            _attackOrderRequest = World.GetRequest<AttackOrderRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _projectileRequest.Consume())
            {
                var gridEntity = _gridFilter.First();
                ref var gridComp = ref _gridStash.Get(gridEntity);

                SpawnProjectile(gridComp, request);
            }

            foreach (var entity in _projectileFilter)
            {
                FlyProjectile(entity, deltaTime);
            }
        }

        private void SpawnProjectile(GridComponent gridComp, ProjectileCastRequest request)
        {
            var spell = request.spell;
            
            var from = gridComp.ToWorldCoord(request.from);
            var to = gridComp.ToWorldCoord(request.to);
            
            from.y += spell.projectileHeight;
            to.y += spell.projectileHeight;
            
            var dir = to - from;
            dir.y = 0f;
            var rot = dir.sqrMagnitude > 0.0001f
                ? Quaternion.LookRotation(dir.normalized)
                : Quaternion.identity;
            
            var obj = Object.Instantiate(spell.projectilePrefab, from, rot);
            
            var entity = World.CreateEntity();
            _transformStash.Add(entity, new TransformComponent(obj.transform));
            _projectileStash.Add(entity, new ProjectileComponent
            (
                from,
                to,
                spell.projectileSpeed,
                spell.swayAmplitude,
                spell.explosionLifetime,
                spell.explosionPrefab,
                request.orders
            ));
        }

        private void FlyProjectile(Entity entity, float deltaTime)
        {
            ref var projComp = ref _projectileStash.Get(entity);
            var tr = _transformStash.Get(entity).value;
            
            var dist = Vector3.Distance(projComp.from, projComp.to);
            projComp.progress += projComp.speed * deltaTime / dist;
            
            var pos = Vector3.Lerp(projComp.from, projComp.to, projComp.progress);
            
            tr.position = pos;

            if (projComp.progress < 1f)
                return;
            
            foreach (var order in projComp.orders)
                _attackOrderRequest.Publish(order);
            
            Object.Destroy(tr.gameObject);
            World.RemoveEntity(entity);
        }
    
        public void Dispose() {}
    }
}
