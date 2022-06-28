using UnityEngine;

public class Missile : WarEntity
{
    private float launchSpeed, blastRadius, damage;
    private TargetPoint target;

    public override bool GameUpdate()
    {
        if (target == null)
        {
            Recycle();
            return false;
        }
        
        float sqrDistance = Vector3.SqrMagnitude(target.Position - transform.position);

        if (sqrDistance <= 0.1f * 0.1f)
        {
            Recycle();
            return false;
        }
        
        transform.LookAt(target.transform);
        transform.position = Vector3.MoveTowards(
            transform.position, 
            target.Position, 
            launchSpeed * Time.deltaTime);
        return true;
    }

    public override void Recycle()
    {
        Game.SpawnMissileExplosion().Initialize(transform.position, blastRadius, damage);
        OriginFactory.Reclaim(this);
    }

    public void Initialize(Vector3 launchPoint, TargetPoint target, float launchSpeed, float blastRadius, float damage)
    {
        transform.position = launchPoint;
        this.target = target;
        this.launchSpeed = launchSpeed;
        this.blastRadius = blastRadius;
        this.damage = damage;
    }
}