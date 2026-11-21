using Features.SpellPack.Components;
using Features.SpellPack.Requests;
using Scellecs.Morpeh;

namespace Features.SpellPack.Systems
{
    public sealed class SpellChangeSystem : ISystem
    {
        public World World { get; set; }

        private Stash<SpellDataComponent> _spellStash;
        private Request<SpellChangeRequest> _spellChangeRequest;
        
        public void OnAwake()
        {
            _spellStash = World.GetStash<SpellDataComponent>();
            _spellChangeRequest = World.GetRequest<SpellChangeRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _spellChangeRequest.Consume())
            {
                if (!_spellStash.Has(request.unit))
                    continue;

                ref var spellData = ref _spellStash.Get(request.unit);
                if (request.spellIndex < 0 || request.spellIndex >= spellData.availableSpells.Count)
                    continue;

                var spell = spellData.availableSpells[request.spellIndex];
                if (spell == null)
                    continue;

                spellData.selectedSpellIndex = request.spellIndex;
                spellData.spellData = spell;
            }
        }
    
        public void Dispose() { }
    }
}
