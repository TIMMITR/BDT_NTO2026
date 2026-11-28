using System;
using Features.BuffPack.Requests;
using Features.DamagePack;
using Features.HealthPack;
using Features.SpeedPack;
using Scellecs.Morpeh;

namespace Features.BuffPack
{
    public static class BuffStatApplier
    {
        public static void Apply(World world, Entity entity, BuffStatType stat, int amount)
        {
            switch (stat)
            {
                case BuffStatType.Health:
                    world.GetRequest<HealthChangeRequest>().Publish(new HealthChangeRequest
                    {
                        entity = entity,
                        amount = amount
                    }, allowNextFrame: true);
                    
                    break;

                case BuffStatType.MaxHealth:
                    var healthStash = world.GetStash<HealthComponent>();
                    
                    if (!healthStash.Has(entity)) 
                        break;
                    
                    ref var health = ref healthStash.Get(entity);
                    health.maxHealth = Math.Max(1, health.maxHealth + amount);
                    health.currentHealth = Math.Min(health.currentHealth, health.maxHealth);
                    
                    break;

                case BuffStatType.Damage:
                    var damageStash = world.GetStash<DamageComponent>();
                    
                    if (damageStash.Has(entity))
                        damageStash.Get(entity).multiplier += amount;
                    
                    break;

                case BuffStatType.Defence:
                    var defenceStash = world.GetStash<DefenceComponent>();
                    
                    if (defenceStash.Has(entity))
                        defenceStash.Get(entity).currentDefence = Math.Max(0, defenceStash.Get(entity).currentDefence + amount);
                    
                    break;

                case BuffStatType.Speed:
                    var speedStash = world.GetStash<SpeedComponent>();
                    
                    if (speedStash.Has(entity))
                        speedStash.Get(entity).currentSpeed = Math.Max(0, speedStash.Get(entity).currentSpeed + amount);
                    
                    break;

                case BuffStatType.MaxSpeed:
                    var stash = world.GetStash<SpeedComponent>();
                    
                    if (!stash.Has(entity)) 
                        break;

                    ref var speed = ref stash.Get(entity);
                    speed.maxSpeed = Math.Max(1, speed.maxSpeed + amount);
                    speed.currentSpeed = Math.Min(speed.currentSpeed, speed.maxSpeed);
                    
                    break;
            }
        }
    }
}
