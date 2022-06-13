using UnityEngine;

public class TileContent : MonoBehaviour
{
    [SerializeField] private TileContentType type;

    public TileContentFactory OriginFactory { get; set; }
    public TileContentType Type => type;

    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}