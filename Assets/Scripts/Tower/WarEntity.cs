using UnityEngine;

public abstract class WarEntity : GameBehavior
{
    public WarFactory OriginFactory { get; set; }

    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}