using Configs;
using Features.CellSelectionPack.Components;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.InputPack.Requests;
using Features.SpeedPack;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.UnitPack.Systems
{
    public class MoveCommandSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _gridFilter;
        
        private Stash<PickedCellComponent> _pickedCellStash;
        private Stash<HoveredCellComponent> _chosenCellStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<GridComponent> _gridStash;
        private Stash<SpeedComponent> _speedStash;
        private Stash<UnitMoveComponent> _unitMoveStash;
        private Stash<CellTileComponent> _tileStash;
        
        private Request<LmbStartRequest> _lmbRequest;
        
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _pickedCellStash = World.GetStash<PickedCellComponent>();
            _chosenCellStash = World.GetStash<HoveredCellComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _gridStash = World.GetStash<GridComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _unitMoveStash = World.GetStash<UnitMoveComponent>();
            _tileStash = World.GetStash<CellTileComponent>();
            
            _lmbRequest = World.GetRequest<LmbStartRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if(_gridFilter.IsEmpty()) return; 
            
            bool lmbClicked = false;
            
            var gridEntity = _gridFilter.First();
            ref var gridComp =  ref _gridStash.Get(gridEntity);
            
            foreach (var clickRequest in _lmbRequest.Consume())
            {
                lmbClicked = true;
            }
            
            if (!lmbClicked || !_chosenCellStash.Has(gridEntity)) return;

            if (!_pickedCellStash.IsEmpty())
            {
                ref var pickedCellComp = ref _pickedCellStash.Get(gridEntity);
                var from = pickedCellComp.cellCoord;
                var to = pickedCellComp.points[^1];
                var unit = gridComp.takenCells[from];

                if (!_unitMoveStash.Has(unit))
                {
                    _unitMoveStash.Add(unit, new UnitMoveComponent(pickedCellComp.points));
                
                    gridComp.takenCells.Remove(pickedCellComp.cellCoord);
                    gridComp.takenCells.Add(to, unit);
                    _tileStash.Get(gridComp.cells[from]).blocked = false;
                    _tileStash.Get(gridComp.cells[to]).blocked = true;
                }
                
                _pickedCellStash.Remove(gridEntity);
            } 
            else
            {
                var coord = _chosenCellStash.Get(gridEntity).cellCoord;
                
                if (!IsPlayerFigure(gridComp, coord, out var speed)) return;
                
                _pickedCellStash.Set(gridEntity, new PickedCellComponent(coord, speed));
            }
        }

        private bool IsPlayerFigure(GridComponent gridComp, CellCoord cellCoord, out int speed)
        {
            if (!gridComp.takenCells.TryGetValue(cellCoord, out var unitEntity))
            {
                speed = 0;
                return false;
            }
            
            ref var unitComp = ref _unitStash.Get(unitEntity);
            speed = _speedStash.Get(unitEntity).currentSpeed;
            
            return unitComp.teamSide == TeamSide.Player;
        }
    
        public void Dispose() {}
    }
}
