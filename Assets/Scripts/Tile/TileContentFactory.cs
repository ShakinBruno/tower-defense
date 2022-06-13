using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class TileContentFactory : ScriptableObject
{
    [SerializeField] private TileContent emptyPrefab;
    [SerializeField] private TileContent obstaclePrefab;

    private Scene contentScene;

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
        TileContent instance = Instantiate(prefab);
        instance.OriginFactory = this;
        MoveToFactoryScene(instance.gameObject);
        return instance;
    }

    private void MoveToFactoryScene(GameObject gameObject)
    {
        if (!contentScene.isLoaded)
        {
            if (Application.isEditor)
            {
                contentScene = SceneManager.GetSceneByName(name);

                if (!contentScene.isLoaded)
                {
                    contentScene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                contentScene = SceneManager.CreateScene(name);
            }
        }

        SceneManager.MoveGameObjectToScene(gameObject, contentScene);
    }
}