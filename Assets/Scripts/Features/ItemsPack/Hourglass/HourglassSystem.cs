using Configs;
using Features.InputPack.Requests;
using Features.ManaPack;
using Features.RaycastPack;
using Features.StatePack.Components;
using Features.UnitPack.Components;
using Global;
using Scellecs.Morpeh;

namespace Features.ItemsPack.Hourglass
{
    public sealed class HourglassSystem : ISystem
    {
        public World World { get; set; }

        private Filter _raycastFilter;
        private Filter _unitsFilter;

        private Stash<RaycastComponent> _raycastStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<SkipTag> _skipTagStash;

        private Singletone<TurnPhaseComponent> _turnPhase;

        private Event<LmbStartEvent> _lmbStartEvent;
        private Request<ManaChangeRequest> _manaChangeRequest;

        private readonly WorldSettingsSo _worldSettings;

        public HourglassSystem(WorldSettingsSo worldSettings)
        {
            _worldSettings = worldSettings;
        }

        public void OnAwake()
        {
            _raycastFilter = World.Filter.With<RaycastComponent>().Build();
            _unitsFilter = World.Filter.With<UnitComponent>().Build();

            _raycastStash = World.GetStash<RaycastComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _skipTagStash = World.GetStash<SkipTag>();

            _turnPhase = new Singletone<TurnPhaseComponent>(World);

            _lmbStartEvent = World.GetEvent<LmbStartEvent>();
            _manaChangeRequest = World.GetRequest<ManaChangeRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var _ in _lmbStartEvent.publishedChanges)
            {
                if (_turnPhase.Comp.phase != TurnPhase.Player) continue;
                if (_raycastFilter.IsEmpty()) return;

                ref var raycastComponent = ref _raycastStash.Get(_raycastFilter.First());
                if (!raycastComponent.hasHit) continue;

                var hourglassView = raycastComponent.hitCollider.gameObject.GetComponent<HourglassView>();
                if (hourglassView == null) continue;

                hourglassView.PlayFlip();

                // _manaChangeRequest.Publish(new ManaChangeRequest
                // {
                //     amount = -_worldSettings.hourglassManaCost
                // });

                foreach (var entity in _unitsFilter)
                {
                    if (_unitStash.Get(entity).teamSide != TeamSide.Enemy) continue;
                    _skipTagStash.Set(entity, new SkipTag());
                }
            }
        }

        public void Dispose() { }
    }
}
