using Features.FightPack.Components;
using Features.UnitPack.Components;
using Global;
using Scellecs.Morpeh;

namespace Features.BusyPack
{
    public sealed class BusyTagSystem : ISystem
    {
        public World World { get; set; }

        private Filter _movingFilter;
        private Filter _attackingFilter;
        private Filter _busyFilter;

        private Stash<BusyTag> _busyStash;
        private Stash<UnitAnimationComponent> _animationStash;
        private Stash<AttackDesireComponent> _attackDesireStash;

        public void OnAwake()
        {
            _movingFilter = World.Filter.With<UnitAnimationComponent>().Without<BusyTag>().Build();
            _attackingFilter = World.Filter.With<AttackDesireComponent>().Without<BusyTag>().Build();
            _busyFilter = World.Filter.With<BusyTag>().Build();

            _busyStash = World.GetStash<BusyTag>();
            _animationStash = World.GetStash<UnitAnimationComponent>();
            _attackDesireStash = World.GetStash<AttackDesireComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var unit in _busyFilter)
                if (!_animationStash.Has(unit) && !_attackDesireStash.Has(unit))
                    _busyStash.Remove(unit);

            MarkBusy(_movingFilter);
            MarkBusy(_attackingFilter);
        }

        private void MarkBusy(Filter filter)
        {
            foreach (var unit in filter)
                if (!_busyStash.Has(unit))
                    _busyStash.Add(unit);
        }

        public void Dispose() { }
    }
}