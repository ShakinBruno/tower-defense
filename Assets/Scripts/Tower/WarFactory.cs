using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class WarFactory : GameObjectFactory
{
    [SerializeField] private Shell shellPrefab;
    [SerializeField] private Missile missilePrefab;
    [SerializeField] private Explosion shellExplosionPrefab;
    [SerializeField] private Explosion missileExplosionPrefab;
    
    public Shell Shell => Get(shellPrefab);
    public Missile Missile => Get(missilePrefab);
    public Explosion ShellExplosion => Get(shellExplosionPrefab);
    public Explosion MissileExplosion => Get(missileExplosionPrefab);

    public void Reclaim(WarEntity entity)
    {
        Destroy(entity.gameObject);
    }
    
    private T Get<T>(T prefab) where T : WarEntity
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
}