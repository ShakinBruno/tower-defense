using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Collider))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Transform arrow;

    private int distance;
    private Tile north, east, south, west;
    private TileContent content;

    private static readonly Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);

    public bool IsAlternative { get; set; }
    public Tile NextOnPath { get; private set; }
    public Vector3 ExitPoint { get; private set; }
    public Direction PathDirection { get; private set; }
    public TileContentType previousType { get; set; }
    
    public TileContent Content
    {
        get => content;
        set
        {
            if (content != null) content.Recycle();
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }
    
    public bool HasPath => distance != int.MaxValue;
    public Tile GrowPathNorth() => GrowPathTo(north, Direction.South);
    public Tile GrowPathEast() => GrowPathTo(east, Direction.West);
    public Tile GrowPathSouth() => GrowPathTo(south, Direction.North);
    public Tile GrowPathWest() => GrowPathTo(west, Direction.East);

    private Tile GrowPathTo(Tile neighbor, Direction direction)
    {
        if (neighbor == null || Content.isWall || neighbor.HasPath) return null;
        neighbor.distance = distance + 1;
        neighbor.NextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return neighbor.Content.BlocksPath ? null : neighbor;
    }

    public void BecomeDestination()
    {
        distance = 0;
        NextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        NextOnPath = null;
    }

    public void ShowPath()
    {
        if (Content.isWall || Content.isDestination || Content.isSpawnPoint)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            NextOnPath == north ? northRotation :
            NextOnPath == east ? eastRotation :
            NextOnPath == south ? southRotation :
            westRotation;
    }

    public static void MakeEastWestNeighbors(Tile east, Tile west)
    {
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors(Tile north, Tile south)
    {
        south.north = north;
        north.south = south;
    }
}