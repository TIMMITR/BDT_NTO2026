using Scellecs.Morpeh;
using UnityEngine;

namespace Features.RaycastPack
{
    public struct RaycastEvent : IEventData
    {
        public RaycastHit hit;
        public Collider hitCollider;
        public Vector3 hitPoint;
    }
}