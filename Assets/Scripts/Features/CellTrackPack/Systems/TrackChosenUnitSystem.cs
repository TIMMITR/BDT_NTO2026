using Configs;
using Features.CellTrackPack.Events;
using Features.GridPack;
using Features.GridPack.Components;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellTrackPack.Systems
{
    public class TrackChosenUnitSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _gridFilter;
        
        private Stash<GridComponent> _gridStash;
        private Stash<UnitComponent> _unitComponentStash;
        
        private Event<CellChooseChangedEvent> _cellChooseChangedEvent;
        private Event<CellChooseEvent> _cellChooseEvent;
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _unitComponentStash = World.GetStash<UnitComponent>();
            
            _cellChooseChangedEvent = World.GetEvent<CellChooseChangedEvent>();
            _cellChooseEvent = World.GetEvent<CellChooseEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var cellChangeEvent in _cellChooseChangedEvent.publishedChanges)
            {
                var gridEntity = _gridFilter.First();
                ref var gridComp = ref _gridStash.Get(gridEntity);

                GridData.TryGetUnit(gridComp, _unitComponentStash, cellChangeEvent.cellCoord, out var unit, out var side);
                    
                _cellChooseEvent.NextFrame(new CellChooseEvent(side));
            }
        }
    
        public void Dispose()
        {
            
        }
    }
}
