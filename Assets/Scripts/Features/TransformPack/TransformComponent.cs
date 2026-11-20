using Scellecs.Morpeh;
using UnityEngine;

namespace Features.TransformPack
{
    public struct TransformComponent : IComponent
    {
        public Transform value;

        public TransformComponent(Transform value)
        {
            this.value = value;
        }
    }
}