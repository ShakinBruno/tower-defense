using UnityEngine;

[CreateAssetMenu]
public class WarFactory : GameObjectFactory
{
    [SerializeField] private Shell shellPrefab;
    [SerializeField] private Explosion explosionPrefab;

    public Shell Shell => Get(shellPrefab);
    public Explosion Explosion => Get(explosionPrefab);

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