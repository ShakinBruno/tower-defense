using UnityEngine;

[SelectionBase]
public class TileContent : MonoBehaviour
{
    [SerializeField] private TileContentType type;

    public TileContentFactory OriginFactory { get; set; }
    public bool isNone => type == TileContentType.None;
    public bool isWall => type == TileContentType.Wall;
    public bool isDestination => type == TileContentType.Destination;
    public bool isObstacle => type == TileContentType.Obstacle;
    public bool isSpawnPoint => type == TileContentType.SpawnPoint;
    public bool isTower => type == TileContentType.Tower;
    public bool BlocksPath => isObstacle || isTower;

    public virtual void GameUpdate()
    {
    }

    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}