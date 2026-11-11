using Scellecs.Morpeh;
using UnityEngine;

namespace Features.RaycastPack
{
    public struct RaycastComponent : IComponent
    {
        public bool hasHit;
        public RaycastHit hit;
        public Collider hitCollider;
        public Vector3 hitPoint;

        public void Reset()
        {
            hasHit = false;
            hit = default;
            hitCollider = null;
            hitPoint = Vector3.zero;
        }
    }
}