using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[Serializable]
public struct EnemyAnimator
{
    private bool hasAppearClip, hasDisappearClip;
    private const float transitionSpeed = 5f;
    private float transitionProgress;
    private PlayableGraph graph;
    private AnimationMixerPlayable mixer;
    private Clip previousClip;
    public Clip CurrentClip { get; private set; }
    public bool IsDone => GetPlayable(CurrentClip).IsDone();

    public enum Clip
    {
        Move,
        Intro,
        Outro,
        Dying,
        Appear,
        Disappear
    }

    public void Configure(Animator animator, EnemyAnimations animations)
    {
        hasAppearClip = animations.Appear;
        hasDisappearClip = animations.Disappear;
        graph = PlayableGraph.Create();
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        mixer = AnimationMixerPlayable.Create(graph, hasAppearClip || hasDisappearClip ? 6 : 4);

        var clip = AnimationClipPlayable.Create(graph, animations.Move);
        clip.Pause();
        mixer.ConnectInput((int)Clip.Move, clip, 0);
        clip = AnimationClipPlayable.Create(graph, animations.Intro);
        clip.SetDuration(animations.Intro.length);
        mixer.ConnectInput((int)Clip.Intro, clip, 0);
        clip = AnimationClipPlayable.Create(graph, animations.Outro);
        clip.SetDuration(animations.Outro.length);
        clip.Pause();
        mixer.ConnectInput((int)Clip.Outro, clip, 0);
        clip = AnimationClipPlayable.Create(graph, animations.Dying);
        clip.SetDuration(animations.Dying.length);
        clip.Pause();
        mixer.ConnectInput((int)Clip.Dying, clip, 0);

        if (hasAppearClip)
        {
            clip = AnimationClipPlayable.Create(graph, animations.Appear);
            clip.SetDuration(animations.Appear.length);
            clip.Pause();
            mixer.ConnectInput((int)Clip.Appear, clip, 0);
        }

        if (hasDisappearClip)
        {
            clip = AnimationClipPlayable.Create(graph, animations.Disappear);
            clip.SetDuration(animations.Disappear.length);
            clip.Pause();
            mixer.ConnectInput((int)Clip.Disappear, clip, 0);
        }

        var output = AnimationPlayableOutput.Create(graph, "Enemy", animator);
        output.SetSourcePlayable(mixer);
    }

    public void GameUpdate()
    {
        if (transitionProgress >= 0f)
        {
            transitionProgress += Time.deltaTime * transitionSpeed;

            if (transitionProgress >= 1f)
            {
                transitionProgress = -1f;
                SetWeight(CurrentClip, 1f);
                SetWeight(previousClip, 0f);
            }
            else
            {
                SetWeight(CurrentClip, transitionProgress);
                SetWeight(previousClip, 1f - transitionProgress);
            }
        }
    }

    public void PlayIntro()
    {
        SetWeight(Clip.Intro, 1f);
        CurrentClip = Clip.Intro;
        graph.Play();
        transitionProgress = -1f;

        if (hasAppearClip)
        {
            GetPlayable(Clip.Appear).Play();
            SetWeight(Clip.Appear, 1f);
        }
    }

    public void PlayMove(float speed)
    {
        GetPlayable(Clip.Move).SetSpeed(speed);
        BeginTransition(Clip.Move);
        if (hasAppearClip) SetWeight(Clip.Appear, 0f);
    }

    public void PlayOutro()
    {
        BeginTransition(Clip.Outro);
        if (hasDisappearClip) PlayDisappearFor(Clip.Outro);
    }

    public void PlayDying()
    {
        BeginTransition(Clip.Dying);
        if (hasDisappearClip) PlayDisappearFor(Clip.Dying);
    }

    public void Stop()
    {
        graph.Stop();
    }

    public void Destroy()
    {
        graph.Destroy();
    }

    private void PlayDisappearFor(Clip otherClip)
    {
        Playable clip = GetPlayable(Clip.Disappear);
        clip.Play();
        clip.SetDelay(GetPlayable(otherClip).GetDuration() - clip.GetDuration());
        SetWeight(Clip.Disappear, 1f);
    }

    private void BeginTransition(Clip nextClip)
    {
        previousClip = CurrentClip;
        CurrentClip = nextClip;
        transitionProgress = 0f;
        GetPlayable(nextClip).Play();
    }

    private void SetWeight(Clip clip, float weight)
    {
        mixer.SetInputWeight((int)clip, weight);
    }

    private Playable GetPlayable(Clip clip)
    {
        return mixer.GetInput((int)clip);
    }
}