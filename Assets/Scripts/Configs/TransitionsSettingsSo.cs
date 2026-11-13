using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.EasyTransitions.Scripts;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "TransitionsSettings", menuName = "Configs/New Transitions Settings")]
    public class TransitionsSettingsSo : ScriptableObject
    {
        [SerializeField] private List<TransitionEntry> transitions = new();

        public TransitionSettings Get(TransitionType type) =>
            transitions.Where(entry => entry.type == type)
                .Select(entry => entry.settings)
                .FirstOrDefault();
    }
    
    [Serializable]
    public struct TransitionEntry
    {
        public TransitionType type;
        public TransitionSettings settings;
    }
    
    public enum TransitionType
    {
        Brush,
        CircleWipe,
        DiagonalRectangleGrid,
        DoubleWipe,
        Fade,
        HorizontalCurtain,
        LinearWipe,
        Noise,
        PaintSplash,
        RectangleGrid,
        RectangleWipe,
        VerticalCurtain
    }
}