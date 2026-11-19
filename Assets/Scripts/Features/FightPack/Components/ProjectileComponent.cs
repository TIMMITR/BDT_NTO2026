using System.Collections.Generic;
using Features.FightPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.FightPack.Components
{
    public struct ProjectileComponent : IComponent
    {
        public Vector3 from;
        public Vector3 to;
        public float progress;
        public float speed;
        public float swayAmplitude;
        public float explosionLifetime;
        public GameObject explosionPrefab;
        public List<AttackOrderRequest> orders;

        public ProjectileComponent(Vector3 from, Vector3 to,
            float speed, float swayAmplitude, float explosionLifetime,
            GameObject explosionPrefab,  List<AttackOrderRequest> orders)
        {
            this.from = from;
            this.to = to;
            this.speed = speed;
            this.swayAmplitude = swayAmplitude;
            this.explosionLifetime = explosionLifetime;
            this.explosionPrefab = explosionPrefab;
            this.orders = orders;
            
            this.progress = 0f;
        }
    }
}
