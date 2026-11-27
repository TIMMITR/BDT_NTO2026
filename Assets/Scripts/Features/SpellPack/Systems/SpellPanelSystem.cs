using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Tags;
using Features.SpellPack.Components;
using Features.SpellPack.Requests;
using Features.SpellPack.View;
using Global;
using Scellecs.Morpeh;

namespace Features.SpellPack.Systems
{
    public sealed class SpellPanelSystem : ISystem
    {
        public World World { get; set; }

        private readonly SpellPanelView _view;

        private Singletone<PlayerSelectionTag> _selection;
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<SpellListComponent> _spellListStash;
        private Request<SpellChangeRequest> _spellChangeRequest;

        private Entity _boundUnit;
        private bool _hasBoundUnit;
        private int _selectedIndex = -1;

        public SpellPanelSystem(SpellPanelView view)
        {
            _view = view;
        }

        public void OnAwake()
        {
            _selection = new Singletone<PlayerSelectionTag>(World);
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _spellListStash = World.GetStash<SpellListComponent>();
            _spellChangeRequest = World.GetRequest<SpellChangeRequest>();
            _view.spellClicked += OnSpellClicked;
            _view.Clear();
        }

        public void OnUpdate(float deltaTime)
        {
            var selection = _selection.Entity;
            if (!_selectedUnitStash.Has(selection))
            {
                Clear();
                return;
            }

            var unit = _selectedUnitStash.Get(selection).unit;
            if (World.IsDisposed(unit) || !_spellListStash.Has(unit))
            {
                Clear();
                return;
            }

            var list = _spellListStash.Get(unit);
            if (_hasBoundUnit && _boundUnit == unit && _selectedIndex == list.currentIndex)
                return;

            _view.Bind(list.spellList, list.currentIndex);
            _boundUnit = unit;
            _hasBoundUnit = true;
            _selectedIndex = list.currentIndex;
        }

        private void OnSpellClicked(int index)
        {
            _spellChangeRequest.Publish(new SpellChangeRequest(index));
        }

        private void Clear()
        {
            if (!_hasBoundUnit)
                return;

            _view.Clear();
            _boundUnit = default;
            _hasBoundUnit = false;
            _selectedIndex = -1;
        }

        public void Dispose()
        {
            if (_view == null)
                return;

            _view.spellClicked -= OnSpellClicked;
            _view.Clear();
        }
    }
}
