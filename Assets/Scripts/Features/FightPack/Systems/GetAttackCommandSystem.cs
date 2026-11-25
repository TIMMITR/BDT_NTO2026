using Features.FightPack.Components;
using Features.FightPack.Requests;
using Features.SpellPack.Components;
using Scellecs.Morpeh;

namespace Features.FightPack.Systems
{
    // система получает реквэсты из системы выбора клеток
    public class GetAttackCommandSystem : ISystem 
    {
        public World World { get; set; }
 
        private Stash<SpellDataComponent> _spellStash;
        private Stash<AttackDesireComponent> _attackDesireStash;
 
        private Request<AttackCommandRequest> _attackCommandRequest;
 
        public void OnAwake()
        {
            _spellStash = World.GetStash<SpellDataComponent>();
            _attackDesireStash = World.GetStash<AttackDesireComponent>();
 
            _attackCommandRequest = World.GetRequest<AttackCommandRequest>();
        }
 
        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _attackCommandRequest.Consume())
            {
                if (!_spellStash.Has(request.unit))
                    continue;
 
                var spell = _spellStash.Get(request.unit).spellData;
                _attackDesireStash.Set(request.unit, new AttackDesireComponent(request.target, spell));
            }
        }
 
        public void Dispose() { }
    }
}
