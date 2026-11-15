using Features.CellGridPack.Components;
using Features.CellPack.Components;
using Features.InputPack.Requests;
using Features.PickCellPack.Requests;
using Scellecs.Morpeh;

namespace Features.UnitPack.Systems
{
    public class MoveUnitSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _gridFilter;
        
        private Stash<CellGridComponent> _gridStash;
        private Stash<PickedCellComponent> _pickedCellStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        
        private Request<CellPickedRequest> _pickedRequest;
        private Request<LmbStartRequest> _lmbRequest;

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<CellGridComponent>().Build();
            
            _gridStash = World.GetStash<CellGridComponent>();
            _pickedCellStash = World.GetStash<PickedCellComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            
            _pickedRequest = World.GetRequest<CellPickedRequest>();
            _lmbRequest = World.GetRequest<LmbStartRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if(_gridFilter.IsEmpty()) return; 
            
            bool lmbClicked = false;
            bool hasPicked = false;
            
            var gridEntity = _gridFilter.First();
            
            foreach (var clickRequest in _lmbRequest.Consume())
            {
                lmbClicked = true;
                hasPicked = _pickedCellStash.Has(gridEntity); // todo потом надо отслеживать availableCellComp
            }
            
            if (!lmbClicked) return;
            
            ref var chosenCellComp = ref _chosenCellStash.Get(gridEntity);
            _pickedCellStash.Set(gridEntity, new PickedCellComponent{cellCoord = chosenCellComp.cellCoord});
        }
    
        public void Dispose() {}
    }
}
