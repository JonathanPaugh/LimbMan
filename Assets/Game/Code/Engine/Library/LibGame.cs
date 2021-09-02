using System.Collections.Generic;
using System.Linq;
using Jape;
using UnityEngine;

public static class GameAnimation
{
    public static AnimationClip GetClip(Animator Animator, string Name)
    {
        return Animator.runtimeAnimatorController.animationClips.FirstOrDefault(C => C.name == Name);
    }
    
    public static AnimatorClipInfo GetCurrentClip(Animator Animator, int Layer = default, int Index = default)
    {
        return Animator.GetCurrentAnimatorClipInfo(Layer)[Index];
    }

    public static AnimatorStateInfo GetCurrentState(Animator Animator, int Layer = default)
    {
        return Animator.GetCurrentAnimatorStateInfo(Layer);
    }

    public static float GetTime(Animator Animator)
    {
        return GetCurrentClip(Animator).clip.length * GetCurrentState(Animator).normalizedTime;
    }

    public static int GetFrame(Animator Animator)
    {
        float Temp = GetCurrentClip(Animator).clip.length * GetCurrentClip(Animator).clip.frameRate * GetCurrentState(Animator).normalizedTime;
        return (int)Temp;
    }

    public static void SetSpeed(Animator Animator, float Speed)
    {
        Animator.SetFloat("Speed", Speed);
    }

    public static void Jump(Animator Animator, float Time)
    {
        AnimationClip Clip = GetCurrentClip(Animator).clip;
        Animator.Play(Clip.name, default, Time);
    }

    public static void Jump(Animator Animator, int Frame)
    {
        AnimationClip Clip = GetCurrentClip(Animator).clip;
        float Time = Frame / (Clip.frameRate * Clip.length);
        Animator.Play(Clip.name, default, Time);
    }

    public static void PlayIdle(Animator Animator)
    {
        Animator.SetFloat("Speed", 1);
        Animator.SetTrigger("Idle");
    }

    public static void PlayAnimation(Animator Animator, string Trigger, float Speed = -1)
    {
        Animator.ResetTrigger("Idle");

        if (!Mathf.Approximately(Speed, -1)) {
            Animator.SetFloat("Speed", Speed);
            Animator.SetTrigger(Trigger);
        } else {
            Animator.SetTrigger(Trigger);
        }
    }

    public static void StartAnimation(Animator Animator, string Trigger, float Speed = -1)
    {
        Animator.ResetTrigger("Idle");

        if (!Mathf.Approximately(Speed, -1)) {
            Animator.SetFloat("Speed", Speed);
            Animator.SetBool(Trigger, true);
        } else {
            Animator.SetBool(Trigger, true);
        }
    }

    public static void StopAnimation(Animator Animator, string Trigger, float Speed = -1)
    {
        Animator.ResetTrigger("Idle");

        if (!Mathf.Approximately(Speed, -1)) {
            Animator.SetFloat("Speed", Speed);
            Animator.SetBool(Trigger, false);
        } else {
            Animator.SetBool(Trigger, false);
        }
    }

    public static void SetOverride(Animator Animator, AnimatorOverrideController Override, AnimationClipOverrides Clips, AnimationClip Clip, string OverrideName)
    {
        Override.GetOverrides(Clips);
        Clips[OverrideName] = Clip;
        Override.ApplyOverrides(Clips);
        Animator.runtimeAnimatorController = Override;
    }

    public static void PlayOverride(Player Unit, AnimationClip Clip, string Trigger, float Speed = -1)
    {
        SetOverride(Unit.animator, Unit.animatorOverrides, Unit.animatorClips, Clip, Trigger);
        PlayAnimation(Unit.animator, Trigger, Speed);
    }

    public static void StartOverride(Player Unit, AnimationClip Clip, string Trigger, float Speed = -1)
    {
        SetOverride(Unit.animator, Unit.animatorOverrides, Unit.animatorClips, Clip, Trigger);
        StartAnimation(Unit.animator, Trigger, Speed);
    }

    public static void StopOverride(Player Unit, AnimationClip Clip, string Trigger, float Speed = -1)
    {
        SetOverride(Unit.animator, Unit.animatorOverrides, Unit.animatorClips, Clip, Trigger);
        StopAnimation(Unit.animator, Trigger, Speed);
    }

    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) {}

        public AnimationClip this[string name]
        {
            get { return Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                {
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
                }
            }
        }
    }
}

public static class GameExtension
{
    public static Transform FindChildDeep(this Transform transform, string name)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(transform);
        while (queue.Count > 0)
        {
            Transform parent = queue.Dequeue();
            if (parent.name == name)
            {
                return parent;
            }
            foreach(Transform child in parent)
            {
                queue.Enqueue(child);
            }
        }
        return null;
    }
}

public static class GameMath
{
    public static float AngleDirection(float position, float target)
    {
        if (position < target) { return 1;  }
        if (position > target) { return -1; }

        return default;
    }

    public static float DirectionFloat(Direction.Horizontal direction)
    {
        if (direction == Jape.Direction.Horizontal.Right) { return 1; }
        if (direction == Jape.Direction.Horizontal.Left) { return -1; }

        return default;
    }

    public static Direction.Horizontal FloatDirection(float direction)
    {
        if (direction > 0) { return Jape.Direction.Horizontal.Right; }
        if (direction < 0) { return Jape.Direction.Horizontal.Left; }

        return default;
    }
}