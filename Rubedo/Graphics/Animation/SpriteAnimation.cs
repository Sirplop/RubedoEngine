using Rubedo.Graphics.Sprites;
using System;

namespace Rubedo.Graphics.Animation;

/// <summary>
/// TODO: I am SpriteSheetAnimation, and I don't have a summary yet.
/// </summary>
public class SpriteAnimation : IAnimation
{
    private readonly TextureAtlas2D _atlas;
    private readonly SpriteAnimationFrame[] _frames;

    public TextureAtlas2D Atlas => _atlas;
    public string Name { get; }
    public ReadOnlySpan<IAnimationFrame> Frames => _frames;
    public int FrameCount => _frames.Length;
    public bool IsLooping { get; }
    public bool IsReversed { get; }
    public bool IsPingPong { get; }

    internal SpriteAnimation(string name, TextureAtlas2D atlas, SpriteAnimationFrame[] frames, bool loops, bool reversed, bool pingPong)
    {
        Name = name;
        this._atlas = atlas;
        _frames = frames;
        IsLooping = loops;
        IsReversed = reversed;
        IsPingPong = pingPong;
    }
}