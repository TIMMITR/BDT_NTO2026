using Configs;
using Features.CellSelectionPack.Components;
using Features.GridPack.Components;
using Features.SpellPack.Components;
using Features.SpellPack.Requests;
using Scellecs.Morpeh;

namespace Features.SpellPack.Systems
{
    public class SpellChangeSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<SpellDataComponent> _spellStash;
        private Stash<SpellListComponent> _spellListStash;
        
        private Request<SpellChangeRequest> _spellChangeRequest;
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _spellStash = World.GetStash<SpellDataComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _spellListStash = World.GetStash<SpellListComponent>();
            
            _spellChangeRequest = World.GetRequest<SpellChangeRequest>();            
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _spellChangeRequest.Consume())
            {
                var gridEntity = _gridFilter.First();

                if (!_selectedUnitStash.Has(gridEntity))
                    continue;
                
                var unit = _selectedUnitStash.Get(gridEntity).unit;

                if (!_spellListStash.Has(unit))
                    continue;
                
                ref var list = ref _spellListStash.Get(unit);

                if (list.currentIndex == request.index && _spellStash.Has(unit))
                {
                    list.currentIndex = -1;
                    _spellStash.Remove(unit);
                }
                else
                {
                    list.currentIndex = request.index;
                    _spellStash.Set(unit, new SpellDataComponent(list.spellList[request.index]));
                }
            }
        }
    
        public void Dispose() { }
    }
}
