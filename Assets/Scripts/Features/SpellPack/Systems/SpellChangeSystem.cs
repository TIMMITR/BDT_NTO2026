using Configs;
using Features.SpellPack.Components;
using Scellecs.Morpeh;

namespace Features.SpellPack.Systems
{
    public class SpellChangeSystem : ISystem //todo пока что просто заглушка, потом нузжно будет норм систему написать
    {
        public World World { get; set; }

        private Stash<SpellDataComponent> _spellStash;

        private readonly SpellConfigSo _spellConfig;

        public SpellChangeSystem(SpellConfigSo spellConfig)
        {
            _spellConfig = spellConfig;
        }
        
        public void OnAwake()
        {
            _spellStash = World.GetStash<SpellDataComponent>();
            
            var spellEnt = World.CreateEntity();
            _spellStash.Add(spellEnt, new SpellDataComponent(_spellConfig));
        }

        public void OnUpdate(float deltaTime)
        {
            
        }
    
        public void Dispose() { }
    }
}
