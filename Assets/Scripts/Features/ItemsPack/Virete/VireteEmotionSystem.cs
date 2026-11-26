using Features.QuickTalkPack.Data;
using Features.QuickTalkPack.Events;
using Features.TypingPack.Components;
using Features.TypingPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.ItemsPack.Virete
{
    public sealed class VireteEmotionSystem : ISystem
    {
        public World World { get; set; }

        private Filter _typingFilter;
        private Stash<TypingComponent> _typingStash;
        private Event<QuickTalkShownEvent> _quickTalkShownEvent;
        private VireteView[] _views;
        private bool _wasTalking;

        public void OnAwake()
        {
            _typingFilter = World.Filter.With<TypingComponent>().Build();
            _typingStash = World.GetStash<TypingComponent>();
            _quickTalkShownEvent = World.GetEvent<QuickTalkShownEvent>();
            _views = Object.FindObjectsByType<VireteView>(FindObjectsSortMode.None);
        }

        public void OnUpdate(float deltaTime)
        {
            var isTalking = IsVireteTalking();
            if (isTalking != _wasTalking)
            {
                _wasTalking = isTalking;
                foreach (var view in _views)
                {
                    if (view != null)
                    {
                        view.SetTalking(isTalking);
                    }
                }
            }

            foreach (var evt in _quickTalkShownEvent.publishedChanges)
            {
                if (evt.set == null || evt.set.Trigger != QuickTalkTrigger.VireteBored) continue;
                if (evt.line.speaker != Speaker.Virete) continue;

                foreach (var view in _views)
                {
                    if (view != null)
                    {
                        view.PlayBored();
                    }
                }
            }
        }

        private bool IsVireteTalking()
        {
            if (_typingFilter.IsEmpty()) return false;

            ref var typing = ref _typingStash.Get(_typingFilter.First());
            return typing is { active: true, line: { speaker: Speaker.Virete } };
        }

        public void Dispose() { }
    }
}
