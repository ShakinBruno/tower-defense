using System;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField] private EnemyConfig small, medium, large;

    [Serializable]
    private class EnemyConfig
    {
        public Enemy prefab;
        [FloatRangeSlider(0.5f, 2f)] public FloatRange scale = new FloatRange(1f);
        [FloatRangeSlider(0.2f, 5f)] public FloatRange speed = new FloatRange(1f);
        [FloatRangeSlider(-0.4f, 0.4f)] public FloatRange pathOffset = new FloatRange(0f);
        [FloatRangeSlider(10f, 1000f)] public FloatRange health = new FloatRange(100f);
        [FloatRangeSlider(1f, 1000f)] public FloatRange prize = new FloatRange(10f);
    }

    public Enemy Get(EnemyType type)
    {
        EnemyConfig config = GetConfig(type);
        Enemy instance = CreateGameObjectInstance(config.prefab);
        instance.OriginFactory = this;
        instance.Initialize(
            config.scale.RandomValueInRange, 
            config.speed.RandomValueInRange, 
            config.pathOffset.RandomValueInRange,
            config.health.RandomValueInRange,
            config.prize.RandomValueInRange);
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    private EnemyConfig GetConfig(EnemyType type)
    {
        return type switch
        {
            EnemyType.Small => small,
            EnemyType.Medium => medium,
            EnemyType.Large => large,
            _ => null
        };
    }
}