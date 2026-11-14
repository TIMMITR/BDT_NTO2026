using System.Collections.Generic;
using Features.TypingPack.Data;
using UnityEngine;

namespace Features.QuickTalkPack.Data
{
    [CreateAssetMenu(fileName = "QuickTalkSet", menuName = "Dialogue/Quick Talk Set", order = 11)]
    public class QuickTalkSet : ScriptableObject
    {
        [SerializeField] private QuickTalkTrigger trigger;
        [SerializeField] private List<DialogueLine> pool = new();
        [SerializeField] private int priority = 10;
        [SerializeField] private float cooldownSeconds = 30f;
        [SerializeField] private bool oncePerBattle;
        [SerializeField] private bool oncePerRun; // todo пока нет сейва работает как oncePerBattle

        public QuickTalkTrigger Trigger => trigger;
        public IReadOnlyList<DialogueLine> Pool => pool;
        public int Priority => priority;
        public float CooldownSeconds => cooldownSeconds;
        public bool OncePerBattle => oncePerBattle;
        public bool OncePerRun => oncePerRun;
    }
}
