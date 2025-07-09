using System;

namespace Rubedo.Graphics.Animation;

/// <summary>
/// TODO: I am IAnimation, and I don't have a summary yet.
/// </summary>
public interface IAnimation
{
    /// <summary>
    /// The name of the animation.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A read-only collection of frames in the animation.
    /// </summary>
    ReadOnlySpan<IAnimationFrame> Frames { get; }

    /// <summary>
    /// The total number of frames in the animation.
    /// </summary>
    int FrameCount { get; }

    /// <summary>
    /// Indicates whether the animation should loop.
    /// </summary>
    bool IsLooping { get; }

    /// <summary>
    /// Indicates whether the animation is reversed.
    /// </summary>
    bool IsReversed { get; }

    /// <summary>
    /// Indicates whether the animation should ping-pong (reverse direction at the ends).
    /// </summary>
    bool IsPingPong { get; }
}