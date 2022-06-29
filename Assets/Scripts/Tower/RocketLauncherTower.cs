using System.Collections;
using UnityEngine;

public class RocketLauncherTower : Tower
{
    [SerializeField] private Transform turret;
    [SerializeField] private float launchSpeed;
    [SerializeField, Range(0.1f, 3f)] private float shotsPerSecond;
    [SerializeField, Range(0.5f, 3f)] private float missileBlastRadius;
    [SerializeField, Range(1f, 100f)] private float missileDamage;
    
    private float launchProgress;
    private TargetPoint target;
    private Coroutine activeCoroutine;
    public override TowerType TowerType => TowerType.RocketLauncher;
    
    public override void GameUpdate()
    {
        launchProgress += shotsPerSecond * Time.deltaTime;

        while (launchProgress >= 1f)
        {
            if (TrackTarget(ref target) || AcquireTarget(out target))
            {
                if (activeCoroutine != null) StopCoroutine(activeCoroutine);
                activeCoroutine = StartCoroutine(Launch(target));
                launchProgress -= 1f;
            }
            else
            {
                launchProgress = 0f;
            }
        }
    }
    
    private IEnumerator RotateToTarget()
    {
        Vector3 direction = target.Position - turret.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(turret.localRotation, lookRotation) > 0.1f)
        {
            turret.localRotation = Quaternion.RotateTowards(
                turret.localRotation, 
                lookRotation, 
                rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
    
    private IEnumerator Launch(TargetPoint currentTarget)
    {
        yield return RotateToTarget();
        Game.SpawnMissile().Initialize(
            turret.position,
            currentTarget, 
            launchSpeed, 
            missileBlastRadius, 
            missileDamage);
    }
}