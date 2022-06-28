using System.Collections;
using UnityEngine;

public class MortarTower : Tower
{
    [SerializeField] private Transform mortar;
    [SerializeField, Range(0.5f, 2f)] private float shotsPerSecond;
    [SerializeField, Range(0.5f, 3f)] private float shellBlastRadius;
    [SerializeField, Range(1f, 100f)] private float shellDamage;

    private float launchSpeed, launchProgress;
    private TargetPoint target;
    private Coroutine activeCoroutine;
    public override TowerType TowerType => TowerType.Mortar;

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        float x = targetingRange + 0.25001f;
        float y = -mortar.position.y;
        launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }

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
                launchProgress = 0.999f;
            }
        }
    }

    private IEnumerator RotateToTarget(Vector2 direction, float tanTheta)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, tanTheta, direction.y));

        while (Quaternion.Angle(mortar.localRotation, lookRotation) > 0.1f)
        {
            mortar.localRotation = Quaternion.RotateTowards(
                mortar.localRotation, 
                lookRotation, 
                rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator Launch(TargetPoint currentTarget)
    {
        Vector3 launchPoint = mortar.position;
        Vector3 targetPoint = currentTarget.Position;
        targetPoint.y = 0f;

        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;
        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= x;
        
        const float g = 9.81f;
        float s = launchSpeed;
        float s2 = s * s;
        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;
        
        yield return RotateToTarget(dir, tanTheta);
        Game.SpawnShell().Initialize(
            launchPoint,
            targetPoint,
            new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y),
            shellBlastRadius,
            shellDamage);
    }
}