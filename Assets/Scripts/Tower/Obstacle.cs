using UnityEngine;

public class Obstacle : TileContent
{
    [SerializeField] private int cost;

    public int Cost => cost;
}