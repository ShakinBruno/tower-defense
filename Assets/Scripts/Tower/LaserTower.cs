using UnityEngine;

public class LaserTower : Tower
{
    [SerializeField] private Transform turret, laserBeam;
    [SerializeField, Range(1f, 100f)] private float damagePerSecond;

    private TargetPoint target;
    private Vector3 laserBeamScale;
    public override TowerType TowerType => TowerType.Laser;
    
    private void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        if ((TrackTarget(ref target) || AcquireTarget(out target)) && RotateToTarget())
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
    }

    private bool RotateToTarget()
    {
        Vector3 direction = target.Position - turret.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        turret.rotation = Quaternion.RotateTowards(
            turret.rotation, 
            lookRotation, 
            rotationSpeed * Time.deltaTime);
        laserBeam.localRotation = turret.localRotation;
        return Quaternion.Angle(turret.rotation, lookRotation) <= 0.1f;
    }
    
    private void Shoot()
    {
        float distance = Vector3.Distance(turret.position, target.Position);
        laserBeamScale.z = distance;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition = turret.localPosition + 0.5f * distance * laserBeam.forward;
        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}