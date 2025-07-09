namespace Rubedo.Graphics.Animation;

/// <summary>
/// TODO: I am SpriteAnimationFrame, and I don't have a summary yet.
/// </summary>
internal class SpriteAnimationFrame : IAnimationFrame
{
    public int FrameIndex { get; }
    public float Duration { get; }

    internal SpriteAnimationFrame(int frame, float duration)
    {
        FrameIndex = frame;
        Duration = duration;
    }
}