using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Collider))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Transform arrow;

    private int distance;
    private Tile north, east, south, west, nextOnPath;
    private TileContent content;

    private static readonly Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);

    public bool IsAlternative { get; set; }

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
    public Tile GrowPathNorth() => GrowPathTo(north);
    public Tile GrowPathEast() => GrowPathTo(east);
    public Tile GrowPathSouth() => GrowPathTo(south);
    public Tile GrowPathWest() => GrowPathTo(west);

    private Tile GrowPathTo(Tile neighbor)
    {
        if (neighbor == null || Content.Type == TileContentType.Wall || neighbor.HasPath) return null;
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        return neighbor.Content.Type != TileContentType.Obstacle ? neighbor : null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void ShowPath()
    {
        if (Content.Type == TileContentType.Wall || Content.Type == TileContentType.Destination)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation :
            westRotation;
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
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