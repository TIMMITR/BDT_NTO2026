using System.Collections.Generic;
using Configs;
using Features.CellSelectionPack.Components;
using Features.DamagePack;
using Features.FightPack.Events;
using Features.GridPack.Components;
using Features.HealthPack;
using Features.OverlayPack.Views;
using Features.SpeedPack;
using Features.SpellPack.Components;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.OverlayPack.Systems
{
    public sealed class BattleUISystem : ISystem
    {
        private readonly IReadOnlyList<HeroSlotView> _heroSlots;
        private readonly IReadOnlyList<SpellSlotView> _spellSlots;
        private readonly List<Entity> _heroes = new();

        public World World { get; set; }

        private Filter _unitFilter;
        private Filter _gridFilter;
        private Filter _turnPhaseFilter;
        private Stash<UnitComponent> _unitStash;
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<HealthComponent> _healthStash;
        private Stash<SpeedComponent> _speedStash;
        private Stash<DefenceComponent> _defenceStash;
        private Stash<DamageComponent> _damageStash;
        private Stash<SpellDataComponent> _spellStash;
        private Stash<TurnPhaseComponent> _turnPhaseStash;
        private Event<PhaseChangedEvent> _phaseChangedEvent;
        private Event<UnitAttackedEvent> _unitAttackedEvent;

        private Entity? _activeUnit;

        public BattleUISystem(IReadOnlyList<HeroSlotView> heroSlots, IReadOnlyList<SpellSlotView> spellSlots)
        {
            _heroSlots = heroSlots;
            _spellSlots = spellSlots;
        }

        public void OnAwake()
        {
            _unitFilter = World.Filter.With<UnitComponent>().Build();
            _gridFilter = World.Filter.With<GridComponent>().Build();
            _turnPhaseFilter = World.Filter.With<TurnPhaseComponent>().Build();

            _unitStash = World.GetStash<UnitComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _healthStash = World.GetStash<HealthComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _defenceStash = World.GetStash<DefenceComponent>();
            _damageStash = World.GetStash<DamageComponent>();
            _spellStash = World.GetStash<SpellDataComponent>();
            _turnPhaseStash = World.GetStash<TurnPhaseComponent>();

            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
            _unitAttackedEvent = World.GetEvent<UnitAttackedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_heroSlots.Count == 0 && _spellSlots.Count == 0)
                return;

            var isPlayerTurn = !_turnPhaseFilter.IsEmpty() &&
                               _turnPhaseStash.Get(_turnPhaseFilter.First()).phase == TurnPhase.Player;

            UpdateHeroSlots(isPlayerTurn);
            var playerTurnStarted = ReadPhaseEvents();
            ReadAttackEvents();

            var activeUnit = GetActiveUnit();
            if (_activeUnit != activeUnit || playerTurnStarted)
            {
                _activeUnit = activeUnit;
                if (activeUnit.HasValue)
                    OpenHeroPanel(activeUnit.Value);
            }

            UpdateHeroStats();
            UpdateSpellSlots(isPlayerTurn);
        }

        private void UpdateHeroSlots(bool canSelect)
        {
            _heroes.Clear();
            foreach (var unit in _unitFilter)
            {
                if (_unitStash.Get(unit).teamSide == TeamSide.Player)
                    _heroes.Add(unit);
            }

            for (var i = 0; i < _heroSlots.Count; i++)
            {
                var slot = _heroSlots[i];
                var hasHero = i < _heroes.Count;
                slot.gameObject.SetActive(hasHero);

                if (hasHero)
                    slot.Bind(_heroes[i], canSelect);
                else
                    slot.Unbind();
            }
        }

        private bool ReadPhaseEvents()
        {
            var playerTurnStarted = false;
            foreach (var evt in _phaseChangedEvent.publishedChanges)
            {
                if (evt.phase != TurnPhase.Player || evt.type != TurnType.Start)
                    continue;

                playerTurnStarted = true;
                foreach (var slot in _heroSlots)
                    slot.ResetForNewTurn();
            }

            return playerTurnStarted;
        }

        private void ReadAttackEvents()
        {
            foreach (var evt in _unitAttackedEvent.publishedChanges)
                OpenHeroPanel(evt.target);
        }

        private Entity? GetActiveUnit()
        {
            if (!_gridFilter.IsEmpty())
            {
                var grid = _gridFilter.First();
                if (_selectedUnitStash.Has(grid))
                {
                    var selected = _selectedUnitStash.Get(grid).unit;
                    if (_unitStash.Has(selected) && _unitStash.Get(selected).teamSide == TeamSide.Player)
                        return selected;
                }
            }

            if (_activeUnit.HasValue && _heroes.Contains(_activeUnit.Value))
                return _activeUnit.Value;

            return _heroes.Count > 0 ? _heroes[0] : null;
        }

        private void OpenHeroPanel(Entity unit)
        {
            foreach (var slot in _heroSlots)
            {
                if (slot.IsBound && slot.Unit == unit)
                    slot.OpenAutomatically();
            }
        }

        private void UpdateHeroStats()
        {
            foreach (var slot in _heroSlots)
            {
                if (!slot.IsBound)
                    continue;

                var unit = slot.Unit;
                if (!_healthStash.Has(unit) || !_speedStash.Has(unit) || !_defenceStash.Has(unit))
                    continue;

                var health = _healthStash.Get(unit);
                var speed = _speedStash.Get(unit);
                var defence = _defenceStash.Get(unit);
                slot.SetStats(
                    health.currentHealth, health.maxHealth,
                    speed.currentSpeed, speed.maxSpeed,
                    defence.currentDefence);
            }
        }

        private void UpdateSpellSlots(bool canSelect)
        {
            for (var i = 0; i < _spellSlots.Count; i++)
            {
                var slot = _spellSlots[i];
                SpellConfigSo spell = null;
                var damageMultiplier = 1;

                if (_activeUnit.HasValue && _spellStash.Has(_activeUnit.Value))
                {
                    var spellData = _spellStash.Get(_activeUnit.Value);
                    if (spellData.availableSpells != null && i < spellData.availableSpells.Count)
                        spell = spellData.availableSpells[i];

                    if (_damageStash.Has(_activeUnit.Value))
                        damageMultiplier = _damageStash.Get(_activeUnit.Value).multiplier;
                }

                if (spell == null)
                {
                    slot.Hide();
                    continue;
                }

                slot.Bind(_activeUnit.Value, spell, i, damageMultiplier, canSelect);
            }
        }

        public void Dispose() { }
    }
}
