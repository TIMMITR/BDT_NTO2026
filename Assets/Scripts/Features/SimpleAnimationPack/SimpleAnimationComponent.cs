using PrimeTween;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.SimpleAnimationPack
{
    public struct SimpleAnimationComponent : IComponent
    {
        public bool isInitialized;
        public Ease ease;
        public float speed;
        public float amplitude;
        public AnimationType animationType;
        public float basePositionY;
        public Transform transform;
        public Tween tween;

    }

    public enum AnimationType
    {
        Rotation,
        Bobbing
    }
}