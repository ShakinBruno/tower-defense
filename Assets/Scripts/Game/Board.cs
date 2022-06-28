using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform tiles;
    [SerializeField] private float overlapBoxSize;

    private const int contentLayerMask = 1 << 6;
    private const int nonEnemyLayerMask = ~(1 << 7);
    private readonly Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    private readonly Queue<Tile> frontier = new Queue<Tile>();
    private readonly List<Tile> spawnPoints = new List<Tile>();
    private readonly List<TileContent> updatingContent = new List<TileContent>();
    private static readonly Collider[] contentBuffer = new Collider[1];
    private TileContentFactory contentFactory;
    public int SpawnPointCount => spawnPoints.Count;

    public void Initialize(TileContentFactory factory)
    {
        contentFactory = factory;

        foreach (Transform child in tiles)
        {
            Vector2Int coordinates = PositionToCoordinates(child.localPosition);
            var tile = child.GetComponent<Tile>();
            TileContent tileContent = GetTileContent(tile);

            tile.IsAlternative = (coordinates.x & 1) == 0;
            if ((coordinates.y & 1) == 0) tile.IsAlternative = !tile.IsAlternative;

            tile.Content = tileContent != null ? tileContent : factory.Get(TileContentType.None);
            tile.Content.OriginFactory = factory;
            if (tile.Content.isSpawnPoint) spawnPoints.Add(tile);
            if (tile.Content.isTower) updatingContent.Add(tile.Content);

            grid.Add(coordinates, tile);
        }

        foreach (Vector2Int coordinates in grid.Keys)
        {
            if (grid.ContainsKey(coordinates + Vector2Int.left))
            {
                Tile.MakeEastWestNeighbors(grid[coordinates], grid[coordinates + Vector2Int.left]);
            }

            if (grid.ContainsKey(coordinates + Vector2Int.down))
            {
                Tile.MakeNorthSouthNeighbors(grid[coordinates], grid[coordinates + Vector2Int.down]);
            }
        }
        
        FindPaths();
    }
    
    public void GameUpdate()
    {
        foreach (TileContent content in updatingContent)
        {
            content.GameUpdate();
        }
    }

    public void BuyObstacle(Tile tile, ref int playerBank)
    {
        if (!tile.Content.isNone) return;
        tile.Content = contentFactory.Get(TileContentType.Obstacle);
        
        if (!FindPaths())
        {
            tile.Content = contentFactory.Get(TileContentType.None);
            FindPaths();
            return;
        }
        
        int obstacleValue = ((Obstacle)tile.Content).Cost;
        
        if (playerBank - obstacleValue >= 0)
        {
            playerBank -= obstacleValue;
        }
        else
        {
            tile.Content = contentFactory.Get(TileContentType.None);
            FindPaths();
        }
    }

    public void SellObstacle(Tile tile, ref int playerBank)
    {
        int obstacleValue = ((Obstacle)tile.Content).Cost;
        tile.Content = contentFactory.Get(TileContentType.None);
        playerBank += Mathf.RoundToInt(obstacleValue * 0.5f);
        FindPaths();
    }

    public void BuyTower(Tile tile, TowerType towerType, ref int playerBank)
    {
        if (!tile.Content.isWall) return;
        tile.Content = contentFactory.Get(towerType);
        int towerValue = ((Tower)tile.Content).Cost;

        if (playerBank - towerValue >= 0)
        {
            updatingContent.Add(tile.Content);
            playerBank -= towerValue;
        }
        else
        {
            tile.Content = contentFactory.Get(TileContentType.Wall);
        }
    }

    public void SellTower(Tile tile, ref int playerBank)
    {
        int towerValue = ((Tower)tile.Content).Cost;
        updatingContent.Remove(tile.Content);
        tile.Content = contentFactory.Get(TileContentType.Wall);
        playerBank += Mathf.RoundToInt(towerValue * 0.5f);
    }

    public Tile GetTile(Ray ray)
    {
        if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, nonEnemyLayerMask)) return null;
        Vector2Int coordinates = PositionToCoordinates(hit.transform.position);
        return grid.ContainsKey(coordinates) ? grid[coordinates] : null;
    }

    public Tile GetSpawnPoint(int index)
    {
        return spawnPoints[index];
    }

    private TileContent GetTileContent(Tile tile)
    {
        int boxSize = Physics.OverlapBoxNonAlloc(
            tile.transform.localPosition + Vector3.up * overlapBoxSize,
            Vector3.one * overlapBoxSize,
            contentBuffer,
            Quaternion.identity,
            contentLayerMask);

        return boxSize > 0 ? contentBuffer[0].transform.GetComponentInParent<TileContent>() : null;
    }

    private bool FindPaths()
    {
        foreach (Tile tile in grid.Values)
        {
            if (tile.Content.isDestination)
            {
                tile.BecomeDestination();
                frontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }

        if (frontier.Count == 0) return false;

        while (frontier.Count > 0)
        {
            Tile tile = frontier.Dequeue();

            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    frontier.Enqueue(tile.GrowPathNorth());
                    frontier.Enqueue(tile.GrowPathSouth());
                    frontier.Enqueue(tile.GrowPathEast());
                    frontier.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    frontier.Enqueue(tile.GrowPathWest());
                    frontier.Enqueue(tile.GrowPathEast());
                    frontier.Enqueue(tile.GrowPathSouth());
                    frontier.Enqueue(tile.GrowPathNorth());
                }
            }
        }

        foreach (Tile tile in grid.Values)
        {
            if (!tile.HasPath) return false;
        }

        foreach (Tile tile in grid.Values)
        {
            tile.ShowPath();
        }

        return true;
    }

    private static Vector2Int PositionToCoordinates(Vector3 position)
    {
        return new Vector2Int
        {
            x = Mathf.RoundToInt(position.x),
            y = Mathf.RoundToInt(position.z)
        };
    }
}