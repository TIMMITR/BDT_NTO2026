using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Tags;
using Features.SpellPack.Components;
using Features.SpellPack.Requests;
using Global;
using Scellecs.Morpeh;

namespace Features.SpellPack.Systems
{
    public class SpellChangeSystem : ISystem
    {
        public World World { get; set; }

        private Singletone<PlayerSelectionTag> _selection;
        
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<SpellDataComponent> _spellStash;
        private Stash<SpellListComponent> _spellListStash;
        
        private Request<SpellChangeRequest> _spellChangeRequest;
        
        public void OnAwake()
        {
            _selection = new Singletone<PlayerSelectionTag>(World);
            
            _spellStash = World.GetStash<SpellDataComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _spellListStash = World.GetStash<SpellListComponent>();
            
            _spellChangeRequest = World.GetRequest<SpellChangeRequest>();            
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _spellChangeRequest.Consume())
            {
                if (!_selectedUnitStash.Has(_selection.Entity))
                    continue;
                
                var unit = _selectedUnitStash.Get(_selection.Entity).unit;

                if (World.IsDisposed(unit) || !_spellListStash.Has(unit))
                    continue;
                
                ref var list = ref _spellListStash.Get(unit);

                if (request.index < 0 || request.index >= list.spellList.Count)
                    continue;

                var spell = list.spellList[request.index];
                if (spell == null)
                    continue;

                if (list.currentIndex == request.index && _spellStash.Has(unit))
                {
                    list.currentIndex = -1;
                    _spellStash.Remove(unit);
                }
                else
                {
                    list.currentIndex = request.index;
                    _spellStash.Set(unit, new SpellDataComponent(spell));
                }
            }
        }
    
        public void Dispose() { }
    }
}
