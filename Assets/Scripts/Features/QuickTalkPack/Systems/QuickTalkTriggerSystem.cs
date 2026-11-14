using System.Collections.Generic;
using System.Linq;
using Configs;
using Features.BattlePack;
using Features.GamePack;
using Features.QuickTalkPack.Components;
using Features.QuickTalkPack.Data;
using Features.QuickTalkPack.Events;
using Scellecs.Morpeh;

namespace Features.QuickTalkPack.Systems
{
    public sealed class QuickTalkTriggerSystem : ISystem
    {
        public World World { get; set; }

        private Filter _battleFilter;
        private Filter _queueFilter;

        private Stash<BattleComponent> _battleStash;
        private Stash<QuickTalkQueueComponent> _queueStash;

        private Event<QuickTalkTriggerEvent> _triggerEvent;

        private readonly GamePlan _gamePlan;
        private readonly WorldSettingsSo _settings;
        private readonly List<QuickTalkSet> _cooldownKeys = new();

        public QuickTalkTriggerSystem(GamePlan gamePlan, WorldSettingsSo settings)
        {
            _gamePlan = gamePlan;
            _settings = settings;
        }

        public void OnAwake()
        {
            _battleFilter = World.Filter.With<BattleComponent>().Build();
            _queueFilter = World.Filter.With<QuickTalkQueueComponent>().Build();

            _battleStash = World.GetStash<BattleComponent>();
            _queueStash = World.GetStash<QuickTalkQueueComponent>();

            _triggerEvent = World.GetEvent<QuickTalkTriggerEvent>();

            if (_queueFilter.IsEmpty())
            {
                _queueStash.Add(World.CreateEntity(), new QuickTalkQueueComponent
                {
                    candidates = new List<QuickTalkCandidate>(),
                    cooldowns = new Dictionary<QuickTalkSet, float>(),
                    saidThisBattle = new HashSet<QuickTalkSet>(),
                    lastLineIndexes = new Dictionary<QuickTalkSet, int>()
                });
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_queueFilter.IsEmpty()) return;

            var queueEntity = _queueFilter.First();
            ref var queue = ref _queueStash.Get(queueEntity);

            TickCooldowns(ref queue, deltaTime);

            foreach (var evt in _triggerEvent.publishedChanges)
            {
                if (evt.trigger == QuickTalkTrigger.BattleStarted)
                    queue.saidThisBattle.Clear();

                var plan = CurrentBattlePlan();
                if (plan == null) continue;

                foreach (var set in plan.QuickTalks)
                {
                    if (set.Trigger != evt.trigger) continue;
                    if ((set.OncePerBattle || set.OncePerRun) && queue.saidThisBattle.Contains(set)) continue;
                    if (queue.cooldowns.ContainsKey(set)) continue;
                    if (IsAlreadyQueued(queue, set)) continue;

                    queue.candidates.Add(new QuickTalkCandidate
                    {
                        set = set,
                        ttlLeft = _settings.quickTalkCandidateTtlSeconds
                    });
                }
            }
        }

        private void TickCooldowns(ref QuickTalkQueueComponent queue, float deltaTime)
        {
            if (queue.cooldowns.Count == 0) return;

            _cooldownKeys.Clear();
            _cooldownKeys.AddRange(queue.cooldowns.Keys);

            foreach (var set in _cooldownKeys)
            {
                var left = queue.cooldowns[set] - deltaTime;

                if (left <= 0f)
                    queue.cooldowns.Remove(set);
                else
                    queue.cooldowns[set] = left;
            }
        }

        private static bool IsAlreadyQueued(in QuickTalkQueueComponent queue, QuickTalkSet set)
        {
            return queue.candidates.Any(candidate => candidate.set == set);
        }

        private BattlePlan CurrentBattlePlan()
        {
            if (_battleFilter.IsEmpty() || _gamePlan == null) return null;

            var battleId = _battleStash.Get(_battleFilter.First()).currentBattleId;

            if (battleId < 0 || battleId >= _gamePlan.BattlePlans.Count)
                return null;

            return _gamePlan.BattlePlans[battleId];
        }

        public void Dispose() { }
    }
}
