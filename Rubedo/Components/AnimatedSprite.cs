using Microsoft.Xna.Framework;
using Rubedo.Graphics.Animation;
using System.Collections.Generic;

namespace Rubedo.Components;

/// <summary>
/// TODO: I am AnimatedSprite, and I don't have a summary yet.
/// </summary>
public class AnimatedSprite : Sprite
{
    public Dictionary<string, SpriteAnimation> Animations { get; private set; }
        = new Dictionary<string, SpriteAnimation>();

    public AnimationController Controller { get; private set; }
    public SpriteAnimation CurrentAnimation => (SpriteAnimation)Controller.Animation;

    public AnimatedSprite(int layerDepth, Color color) : base(layerDepth, color) { }
    public AnimatedSprite(string startAnimation, int layerDepth, Color color) : base(layerDepth, color)
    {
        AddAnimation(startAnimation, true);
        Controller.Play();
    }
    public AnimatedSprite(SpriteAnimation startAnimation, int layerDepth, Color color) : base(layerDepth, color)
    {
        AddAnimation(startAnimation, true);
        Controller.Play();
    }

    public bool AddAnimation(string animationPath, bool setAsCurrent = false)
    {
        SpriteAnimation animation = Assets.LoadAnimation<SpriteAnimation>(animationPath);
        if (Animations.ContainsKey(animation.Name))
            return false;
        Animations.Add(animation.Name, animation);
        if (setAsCurrent)
        {
            SetAnimation(animation.Name);
        }
        return true;
    }

    public bool AddAnimation(SpriteAnimation animation, bool setAsCurrent = false)
    {
        if (Animations.ContainsKey(animation.Name))
            return false;
        Animations.Add(animation.Name, animation);
        if (setAsCurrent)
        {
            SetAnimation(animation.Name);
        }
        return true;
    }

    public void SetAnimation(string name)
    {
        if (!Animations.TryGetValue(name, out var animation))
        {
            throw new KeyNotFoundException($"Animations '{name}' not found in animation list!");
        }
        Controller = new AnimationController(animation);
        this.SetTexture(CurrentAnimation.Atlas.GetRegion(Controller.CurrentFrame));
    }

    public void ForceUpdateTexture()
    {
        this.SetTexture(CurrentAnimation.Atlas.GetRegion(Controller.CurrentFrame));
    }

    public override void Update()
    {
        if (Controller == null)
            return;

        int index = Controller.CurrentFrame;
        Controller.Update();
        if (index != Controller.CurrentFrame)
            this.SetTexture(CurrentAnimation.Atlas.GetRegion(Controller.CurrentFrame));
    }
}