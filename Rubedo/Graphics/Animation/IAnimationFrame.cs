namespace Rubedo.Graphics.Animation;

/// <summary>
/// TODO: I am IAnimationFrame, and I don't have a summary yet.
/// </summary>
public interface IAnimationFrame
{
    /// <summary>
    /// The index in the sprite atlas of this animation.
    /// </summary>
    public int FrameIndex { get; }
    /// <summary>
    /// The duration this frame is held.
    /// </summary>
    public float Duration { get; }
}