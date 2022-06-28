using UnityEngine;

public class Explosion : WarEntity
{
    [SerializeField, Range(0f, 1f)] private float duration;

    private float age;

    public void Initialize(Vector3 position, float blastRadius, float damage)
    {
        TargetPoint.FillBuffer(position, blastRadius);
        
        for (var i = 0; i < TargetPoint.BufferedCount; i++)
        {
            TargetPoint.GetBuffered(i).Enemy.ApplyDamage(damage);
        }

        transform.localPosition = position;
        transform.localScale = Vector3.one * (2f * blastRadius);
    }

    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        
        if (age >= duration)
        {
            OriginFactory.Reclaim(this);
            return false;
        }

        return true;
    }
}