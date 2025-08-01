using System;
using System.Collections.Generic;

namespace Rubedo.Graphics.Animation;

/// <summary>
/// Represents an instance of an animation.
/// </summary>
public class AnimationInstance : IDisposable
{
    private readonly IAnimation _animation;
    private int _indexDirection;
    private int _currentFrame;
    private bool _disposed = false;

    public IAnimation Animation => _animation;

    public bool Disposed => _disposed;

    /// <summary>
    /// Indicates whether this animation should abide by the time scale.
    /// </summary>
    public bool UseRealTime { get; set; }
    public bool IsPaused { get; protected set; }
    public bool IsAnimating { get; private set; }

    public bool IsLooping { get; set; }
    public bool IsReversed
    {
        get => _indexDirection == -1;
        set => _indexDirection = value ? -1 : 1;
    }
    public bool IsPingPong { get; set; }
    public float Speed { get; set; }
    public float FrameTime { get; private set; }
    public int CurrentFrame => _animation.Frames[_currentFrame].FrameIndex;
    public int FrameCount => _animation.FrameCount;

    public event Action<AnimationInstance, AnimationEvent.Trigger> OnAnimationEvent;


    public AnimationInstance(IAnimation animation)
    {
        _animation = animation;

        IsLooping = animation.IsLooping;
        IsReversed = animation.IsReversed;
        IsPingPong = animation.IsPingPong;
        Speed = 1.0f;
    }

    public void Pause(bool resetDuration = false)
    {
        if (!IsAnimating || IsPaused)
            return; //already paused or is not animating.

        IsPaused = true;
        if (resetDuration)
        {
            FrameTime = _animation.Frames[_currentFrame].Duration;
        }
    }

    public void Unpause(bool advanceFrame = false)
    {
        if (!IsAnimating || !IsPaused)
            return; //not paused, or is not animating.

        IsPaused = false;
        if (advanceFrame)
            AdvanceFrame();
    }

    public bool Play(int startingFrame = 0, bool startOnRandomFrame = false)
    {
        if (!startOnRandomFrame && (startingFrame < 0 || startingFrame >= FrameCount))
            throw new ArgumentOutOfRangeException(nameof(startingFrame), $"{nameof(startingFrame)} cannot be less than zero or greater than the frame count of this {nameof(AnimationInstance)}.");

        if (IsAnimating)
            return false; //already playing.

        IsAnimating = true;
        SetFrame(startOnRandomFrame ? Lib.Random.Range(0, FrameCount) : startingFrame);
        return true;
    }

    public bool Stop() => Stop(AnimationEvent.Trigger.AnimationStopped);
    public bool Stop(AnimationEvent.Trigger trigger)
    {
        if (!IsAnimating)
            return false;

        IsAnimating = false;
        IsPaused = true;
        OnAnimationEvent?.Invoke(this, trigger);
        return true;
    }

    public void SetFrame(int frame)
    {
        if (frame < 0 || frame >= _animation.FrameCount)
        {
            throw new ArgumentOutOfRangeException(nameof(frame), $"{nameof(frame)} cannot be less than zero or greater than the frame count of this {nameof(AnimationInstance)}.");
        }

        _currentFrame = frame;
        FrameTime = _animation.Frames[_currentFrame].Duration;
        OnAnimationEvent?.Invoke(this, AnimationEvent.Trigger.FrameBegin);
    }

    public void Reset()
    {
        IsReversed = _animation.IsReversed;
        IsPingPong = _animation.IsPingPong;
        IsLooping = _animation.IsLooping;
        IsAnimating = false;
        IsPaused = true;
        Speed = 1.0f;
        _currentFrame = IsReversed ? _animation.FrameCount - 1 : 0;
        FrameTime = _animation.Frames[_currentFrame].Duration;
    }

    public void Update()
    {
        if (!IsAnimating || IsPaused)
            return; //we're not animating.

        FrameTime -= (UseRealTime ? (float)Time.RawDeltaTime : Time.DeltaTime) * Speed;

        float remainingTime = 0;
        while (FrameTime < 0)
        { //make sure we can skip frames due to low framerate.
            remainingTime += -FrameTime;

            OnAnimationEvent?.Invoke(this, AnimationEvent.Trigger.FrameEnd);

            if (!AdvanceFrame())
                break;
            FrameTime -= remainingTime;
            remainingTime = 0;
        }
    }
    private bool AdvanceFrame()
    {
        _currentFrame += _indexDirection;
        if (_currentFrame < 0 || _currentFrame >= _animation.FrameCount)
        {
            if (IsLooping)
            {
                if (IsPingPong)
                {
                    _indexDirection = -_indexDirection;
                    _currentFrame += _indexDirection * 2;
                }
                else
                {
                    _currentFrame = IsReversed ? _animation.FrameCount - 1 : 0;
                }
                OnAnimationEvent?.Invoke(this, AnimationEvent.Trigger.AnimationLoop);
            }
            else
            {
                _currentFrame -= _indexDirection;
                Stop(AnimationEvent.Trigger.AnimationCompleted);
                return false;
            }
        }
        FrameTime = _animation.Frames[_currentFrame].Duration;
        OnAnimationEvent?.Invoke(this, AnimationEvent.Trigger.FrameBegin);
        return true;
    }


    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}