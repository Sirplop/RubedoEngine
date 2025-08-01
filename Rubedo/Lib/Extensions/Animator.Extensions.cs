using Rubedo.Components;
using Rubedo.Graphics.Animation;
using Rubedo.Lib.StateMachine;
using System.Collections.Generic;

namespace Rubedo.Lib.Extensions;

/// <summary>
/// Extensions for Animators, primarily for constructing commonly used animation styles.
/// </summary>
public static class AnimatorExtensions
{
    /// <summary>
    /// Creates an animator with a single state of the given sprite animation.
    /// </summary>
    /// <param name="animation">The animation to play.</param>
    /// <param name="speed">The speed at which to play the animation.</param>
    /// <param name="target">The sprite component to target with this animation.</param>
    public static Animator CreateSpriteAnimation(string animation, float speed, Sprite target)
        => CreateSpriteAnimation(new AnimationInstance(Assets.LoadAnimation<SpriteAnimation>(animation)), speed, target);

    /// <summary>
    /// Creates an animator with a single state of the given sprite animation.
    /// </summary>
    /// <param name="animation">The animation to play.</param>
    /// <param name="speed">The speed at which to play the animation.</param>
    /// <param name="target">The sprite component to target with this animation.</param>
    public static Animator CreateSpriteAnimation(AnimationInstance animation, float speed, Sprite target)
    {
        animation.Speed = speed;
        var dict = new Dictionary<string, AnimationInstance>() { { "anim", animation } };
        Fsm<string, string> fsm = Fsm<string, string>.Builder("anim")
            .State("anim")
                .AnimateTexture(target, animation)
            .Build();
        return new Animator(dict, fsm);
    }
}