using System;

namespace Rubedo.Graphics.Animation;

/// <summary>
/// Represents an event that occurs during an animation.
/// </summary>
public class AnimationEvent : EventArgs
{
    public enum Trigger
    {
        /// <summary>
        /// Triggered at the beginning of a frame.
        /// </summary>
        FrameBegin,

        /// <summary>
        /// Triggered at the end of a frame.
        /// </summary>
        FrameEnd,

        /// <summary>
        /// Triggered when a frame is skipped.
        /// </summary>
        FrameSkipped,

        /// <summary>
        /// Triggered when the animation loops.
        /// </summary>
        AnimationLoop,

        /// <summary>
        /// Triggered when the animation completes.
        /// </summary>
        AnimationCompleted,

        /// <summary>
        /// Triggered when the animation stops.
        /// </summary>
        AnimationStopped,
    }

    /// <summary>
    /// Gets the animation controller associated with the event.
    /// </summary>
    public AnimationInstance Animation { get; }

    /// <summary>
    /// Gets the trigger that caused the event.
    /// </summary>
    public Trigger ATrigger { get; }

    internal AnimationEvent(AnimationInstance animation, Trigger trigger) => (Animation, ATrigger) = (animation, trigger);
}