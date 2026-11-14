using Features.CellGridPack.Components;
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
        
        private Request<CellPickedRequest> _pickedRequest;
        private Request<LmbStartRequest> _lmbRequest;

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<CellGridComponent>().Build();
            
            _gridStash = World.GetStash<CellGridComponent>();
            
            _pickedRequest = World.GetRequest<CellPickedRequest>();
            _lmbRequest = World.GetRequest<LmbStartRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            bool lmbClicked = false;
            
            foreach (var clickRequest in _lmbRequest.Consume())
                lmbClicked = true;
            
            foreach (var pickRequest in _pickedRequest.Consume())
            {
                if (lmbClicked)
                {
                
                }
            }
        }
    
        public void Dispose() {}
    }
}
