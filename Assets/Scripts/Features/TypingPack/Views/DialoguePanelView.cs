using Features.TypingPack.Components;
using Features.TypingPack.Data;
using Features.TypingPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Features.TypingPack.Views
{
    public class DialoguePanelView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text speakerLabel;
        [SerializeField] private Text lineLabel;

        private Filter _filter;
        private Stash<TypingComponent> _typingStash;

        private void Start()
        {
            if (World.Default == null) return;

            _filter = World.Default.Filter.With<TypingComponent>().Build();
            _typingStash = World.Default.GetStash<TypingComponent>();
            
            root.SetActive(false);
        }

        private void Update()
        {
            if (_filter == null || _filter.IsEmpty()) return;

            var entity = _filter.First();
            ref var typing = ref _typingStash.Get(entity);

             if (root.activeSelf != typing.active)
                 root.SetActive(typing.active);
            
             if (!typing.active) return;
            
             if (speakerLabel != null)
                 speakerLabel.text = SpeakerName(typing.line.speaker);
            
             if (lineLabel != null)
             {
                 var text = typing.line.text ?? string.Empty;
                 var count = Mathf.Clamp((int)typing.visibleChars, 0, text.Length);
                 lineLabel.text = text[..count];
            }
        }

        public void OnPanelClicked()
        {
            if (World.Default == null) return;
            World.Default.GetRequest<TypingSkipRequest>().Publish(new TypingSkipRequest());
        }

        private static string SpeakerName(Speaker speaker)
        {
            return speaker switch
            {
                Speaker.Warlock => "Колдун",
                Speaker.Verity => "Верити",
                _ => string.Empty
            };
        }
    }
}
