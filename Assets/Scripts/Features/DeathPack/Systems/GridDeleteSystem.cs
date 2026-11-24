using Features.GridPack;
using Features.GridPack.Components;
using Global;
using Scellecs.Morpeh;

namespace Features.DeathPack.Systems
{
    // система отвечает за удаление сущности из сетки
    public class GridDeleteSystem : ISystem // todo переменовать
    {
        public World World { get; set; }

        private Filter _deadFilter;
        private Singletone<GridComponent> _grid;

        private Stash<CellTileComponent> _cellTileStash;
        private Stash<CellPositionComponent> _cellPositionStash;

        public void OnAwake()
        {
            _deadFilter = World.Filter.
                With<DeadTag>().
                With<CellPositionComponent>().
                Build();

            _grid = new Singletone<GridComponent>(World);
            
            _cellTileStash = World.GetStash<CellTileComponent>();
            _cellPositionStash = World.GetStash<CellPositionComponent>();
            
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var unit in _deadFilter)
            {
                GridOccupancy.Remove(ref _grid.Comp, _cellTileStash, unit, _cellPositionStash.Get(unit).coord);
                _cellPositionStash.Remove(unit);
            }
        }
    
        public void Dispose() {}
    }
}
