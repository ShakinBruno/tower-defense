using UnityEngine;

[CreateAssetMenu]
public class EnemyAnimations : ScriptableObject
{
    [SerializeField] private AnimationClip move, intro, outro, dying, appear, disappear;
    [SerializeField] private float moveAnimationSpeed = 1f;

    public float MoveAnimationSpeed => moveAnimationSpeed;
    public AnimationClip Move => move;
    public AnimationClip Intro => intro;
    public AnimationClip Outro => outro;
    public AnimationClip Dying => dying;
    public AnimationClip Appear => appear;
    public AnimationClip Disappear => disappear;
}