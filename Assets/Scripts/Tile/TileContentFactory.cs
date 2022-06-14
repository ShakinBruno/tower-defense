using UnityEngine;

[CreateAssetMenu]
public class TileContentFactory : GameObjectFactory
{
    [SerializeField] private TileContent emptyPrefab;
    [SerializeField] private TileContent obstaclePrefab;

    public TileContent Get(TileContentType type)
    {
        return type switch
        {
            TileContentType.Empty => Get(emptyPrefab),
            TileContentType.Obstacle => Get(obstaclePrefab),
            _ => null
        };
    }

    public void Reclaim(TileContent content)
    {
        Destroy(content.gameObject);
    }

    private TileContent Get(TileContent prefab)
    {
        TileContent instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
}