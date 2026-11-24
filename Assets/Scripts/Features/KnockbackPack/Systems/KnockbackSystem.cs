using System.Collections.Generic;
using Features.DamagePack;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.KnockbackPack.Requests;
using Features.UnitPack.Components;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.KnockbackPack.Systems
{
    public class KnockbackSystem : ISystem
    {
        public World World { get; set; }

        private Singletone<GridComponent> _grid;
        
        private Stash<CellPositionComponent> _positionStash;
        private Stash<CellTileComponent> _cellTileStash;
        private Stash<UnitAnimationComponent> _unitAnimationStash;

        private Request<KnockbackRequest> _knockbackRequest;
        private Request<DamageRequest> _damageRequest;
        
        public void OnAwake()
        {
            _grid = new Singletone<GridComponent>(World);
            
            _positionStash = World.GetStash<CellPositionComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            _unitAnimationStash = World.GetStash<UnitAnimationComponent>();
            
            _knockbackRequest = World.GetRequest<KnockbackRequest>();
            _damageRequest = World.GetRequest<DamageRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _knockbackRequest.Consume())
            {
                var unit = request.unit;

                if (!_positionStash.Has(unit))
                    continue;
                
                var from = _positionStash.Get(unit).coord;
                var dir = request.dir;
                var distance = request.distance;
                var returnDistance = request.returnDistance;
                
                var path = BuildKnockbackPath(from, dir, distance, returnDistance, out var collided);

                if (collided)
                {
                    _damageRequest.Publish(new DamageRequest(request.source, unit, request.damage));
                    
                    Debug.Log($"{request.source} punched {request.source} with {request.damage}");
                }
                
                if (path.Count < 2) 
                    continue;
                
                GridOccupancy.Move(ref _grid.Comp, _cellTileStash, _positionStash, unit, from, path[^1]);
                _unitAnimationStash.Set(unit, new UnitAnimationComponent(path, true));
            }
        }

        private List<CellCoord> BuildKnockbackPath(CellCoord from, CellCoord dir,
            int distance, int returnDistance, out bool collided)
        {
            List<CellCoord> path = new() { from };
            var cell = from;
            collided = false;

            for (int i = 0; i < distance; i++)
            {
                var next = cell + dir;

                if (!_grid.Comp.cells.TryGetValue(next, out var cellEntity))
                    break;

                if (!GridData.IsFreeCell(_grid.Comp, _cellTileStash, next))
                {
                    collided = true;
                    break;
                }
                
                path.Add(next);
                cell = next;
            }

            if (collided)
            {
                for (var i = 0; i < returnDistance; i++)
                {
                    var next = cell - dir;

                    if (!IsFree(next, from))
                    {
                        dir = new CellCoord(-dir.Q, -dir.R);
                        next = cell + dir;
                        
                        if (!IsFree(next, from))
                            break;
                    }
                    
                    path.Add(next);
                    cell = next;
                }
            }
            
            return path;    
        }
        
        private bool IsFree(CellCoord cell, CellCoord self) => cell == self 
            ? !GridData.HasBuilding(_grid.Comp, _cellTileStash, cell) 
            : GridData.IsFreeCell(_grid.Comp, _cellTileStash, cell);
        
        public void Dispose() { }
    }
}
