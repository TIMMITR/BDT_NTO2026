using Features.SpellPack.Requests;
using Features.SpellPack.View;
using Scellecs.Morpeh;

namespace Features.SpellPack.Systems
{
    public sealed class SpellPanelSystem : ISystem
    {
        public World World { get; set; }

        private readonly SpellPanelView _view;
        private Request<SpellChangeRequest> _spellChangeRequest;

        public SpellPanelSystem(SpellPanelView view)
        {
            _view = view;
        }

        public void OnAwake()
        {
            _spellChangeRequest = World.GetRequest<SpellChangeRequest>();
            _view.spellClicked += OnSpellClicked;
        }

        public void OnUpdate(float deltaTime) { }

        private void OnSpellClicked(int index)
        {
            _spellChangeRequest.Publish(new SpellChangeRequest(index));
        }

        public void Dispose()
        {
            if (_view != null)
                _view.spellClicked -= OnSpellClicked;
        }
    }
}
