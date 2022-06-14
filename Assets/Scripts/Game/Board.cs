using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform tiles;
    [SerializeField] private LayerMask contentMask;
    [SerializeField] private float overlapBoxSize;
    [SerializeField] private bool showPaths;

    private readonly Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    private readonly Queue<Tile> frontier = new Queue<Tile>();
    private readonly List<Tile> spawnPoints = new List<Tile>();
    private readonly Collider[] contentCollider = new Collider[1];
    private TileContentFactory contentFactory;

    public int SpawnPointCount => spawnPoints.Count;

    private void Start()
    {
        FindPaths();
    }

    private void Update()
    {
        if (showPaths) // TODO: remove showing path when everything works well
        {
            foreach (Tile tile in grid.Values)
            {
                tile.ShowPath();
            }
        }
        else
        {
            foreach (Tile tile in grid.Values)
            {
                tile.HidePath();
            }
        }
    }

    public void Initialize(TileContentFactory factory)
    {
        contentFactory = factory;

        foreach (Transform child in tiles)
        {
            Vector2Int coordinates = PositionToCoordinates(child.position);
            var tile = child.GetComponent<Tile>();
            TileContent tileContent = GetTileContent(tile);

            tile.IsAlternative = (coordinates.x & 1) == 0;
            if ((coordinates.y & 1) == 0) tile.IsAlternative = !tile.IsAlternative;

            tile.Content = tileContent != null ? tileContent : factory.Get(TileContentType.Empty);
            if (tile.Content.Type == TileContentType.SpawnPoint) spawnPoints.Add(tile);

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
    }

    public void ToggleObstacle(Tile tile)
    {
        if (tile.Content.Type == TileContentType.Obstacle)
        {
            tile.Content = contentFactory.Get(TileContentType.Empty);
            FindPaths();
        }
        else if (tile.Content.Type == TileContentType.Empty)
        {
            tile.Content = contentFactory.Get(TileContentType.Obstacle);

            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(TileContentType.Empty);
                FindPaths();
            }
        }
    }

    public Tile GetTile(Ray ray)
    {
        if (!Physics.Raycast(ray, out RaycastHit hit)) return null;
        Vector2Int coordinates = PositionToCoordinates(hit.point);
        return grid.ContainsKey(coordinates) ? grid[coordinates] : null;
    }

    public Tile GetSpawnPoint(int index)
    {
        return spawnPoints[index];
    }

    private TileContent GetTileContent(Tile tile)
    {
        int boxSize = Physics.OverlapBoxNonAlloc(
            tile.transform.position + Vector3.up * overlapBoxSize,
            Vector3.one * overlapBoxSize,
            contentCollider,
            Quaternion.identity,
            contentMask);

        return boxSize > 0 ? contentCollider[0].transform.GetComponentInParent<TileContent>() : null;
    }

    private bool FindPaths()
    {
        foreach (Tile tile in grid.Values)
        {
            if (tile.Content.Type == TileContentType.Destination)
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

        if (showPaths)
        {
            foreach (Tile tile in grid.Values)
            {
                tile.ShowPath();
            }
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