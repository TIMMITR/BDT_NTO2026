using System.Collections.Generic;
using System.Linq;
using Configs;
using Features.QuickTalkPack.Components;
using Features.QuickTalkPack.Data;
using Features.QuickTalkPack.Events;
using Features.StoryTalkPack.Events;
using Features.TypingPack.Components;
using Features.TypingPack.Data;
using Features.TypingPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.QuickTalkPack.Systems
{
    public sealed class QuickTalkQueueSystem : ISystem
    {
        public World World { get; set; }

        private Filter _queueFilter;
        private Filter _typingFilter;

        private Stash<QuickTalkQueueComponent> _queueStash;
        private Stash<TypingComponent> _typingStash;

        private Request<TypingStartRequest> _typingStartRequest;

        private Event<QuickTalkShownEvent> _shownEvent;
        private Event<StoryTalkStartEvent> _storyTalkStartedEvent;
        private Event<StoryTalkFinishEvent> _storyTalkFinishedEvent;

        private readonly WorldSettingsSo _settings;

        private bool _storyTalkActive;

        public QuickTalkQueueSystem(WorldSettingsSo settings)
        {
            _settings = settings;
        }

        public void OnAwake()
        {
            _queueFilter = World.Filter.With<QuickTalkQueueComponent>().Build();
            _typingFilter = World.Filter.With<TypingComponent>().Build();

            _queueStash = World.GetStash<QuickTalkQueueComponent>();
            _typingStash = World.GetStash<TypingComponent>();

            _typingStartRequest = World.GetRequest<TypingStartRequest>();

            _shownEvent = World.GetEvent<QuickTalkShownEvent>();
            _storyTalkStartedEvent = World.GetEvent<StoryTalkStartEvent>();
            _storyTalkFinishedEvent = World.GetEvent<StoryTalkFinishEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var _ in _storyTalkStartedEvent.publishedChanges)
                _storyTalkActive = true;

            foreach (var _ in _storyTalkFinishedEvent.publishedChanges)
                _storyTalkActive = false;

            if (_queueFilter.IsEmpty()) return;

            var queueEntity = _queueFilter.First();
            ref var queue = ref _queueStash.Get(queueEntity);

            TickCandidateTtl(queue, deltaTime);

            if (queue.globalCooldownLeft > 0f)
                queue.globalCooldownLeft -= deltaTime;

            if (queue.candidates.Count == 0) return;
            if (queue.globalCooldownLeft > 0f) return;
            if (_storyTalkActive) return;
            if (IsPanelBusy()) return;

            var best = TakeBestCandidate(ref queue);
            var line = PickLine(ref queue, best);

            _typingStartRequest.Publish(new TypingStartRequest { line = line });

            queue.globalCooldownLeft = _settings.quickTalkGlobalCooldownSeconds;
            queue.cooldowns[best] = best.CooldownSeconds;

            if (best.OncePerBattle || best.OncePerRun)
                queue.saidThisBattle.Add(best);

            _shownEvent.NextFrame(new QuickTalkShownEvent { set = best, line = line });
            // todo чтобы battle UI снова включался
        }

        private static void TickCandidateTtl(in QuickTalkQueueComponent queue, float deltaTime)
        {
            for (var i = queue.candidates.Count - 1; i >= 0; i--)
            {
                queue.candidates[i].ttlLeft -= deltaTime;

                if (queue.candidates[i].ttlLeft <= 0f)
                    queue.candidates.RemoveAt(i);
            }
        }

        private bool IsPanelBusy()
        {
            return !_typingFilter.IsEmpty() && _typingStash.Get(_typingFilter.First()).active;
        }

        private static QuickTalkSet TakeBestCandidate(ref QuickTalkQueueComponent queue)
        {
            QuickTalkSet best = null;

            foreach (var candidate in queue.candidates.Where(candidate =>
                         best == null || candidate.set.Priority > best.Priority))
                best = candidate.set;

            queue.candidates.Clear();
            return best;
        }

        private static DialogueLine PickLine(ref QuickTalkQueueComponent queue, QuickTalkSet set)
        {
            var pool = set.Pool;
            var last = queue.lastLineIndexes.GetValueOrDefault(set, -1);

            var pick = 0;
            if (pool.Count > 1)
            {
                do
                {
                    pick = Random.Range(0, pool.Count);
                } while (pick == last);
            }

            queue.lastLineIndexes[set] = pick;
            return pool[pick];
        }

        public void Dispose() { }
    }
}
