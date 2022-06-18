using UnityEngine;

[CreateAssetMenu]
public class TileContentFactory : GameObjectFactory
{
    [SerializeField] private TileContent emptyPrefab;
    [SerializeField] private TileContent wallPrefab;
    [SerializeField] private TileContent obstaclePrefab;
    [SerializeField] private Tower[] towerPrefabs;

    public TileContent Get(TileContentType type)
    {
        return type switch
        {
            TileContentType.None => Get(emptyPrefab),
            TileContentType.Wall => Get(wallPrefab),
            TileContentType.Obstacle => Get(obstaclePrefab),
            _ => null
        };
    }

    public Tower Get(TowerType type)
    {
        Tower prefab = towerPrefabs[(int)type - 1];
        return Get(prefab);
    }

    public void Reclaim(TileContent content)
    {
        Destroy(content.gameObject);
    }
    
    private T Get<T>(T prefab) where T : TileContent
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
}