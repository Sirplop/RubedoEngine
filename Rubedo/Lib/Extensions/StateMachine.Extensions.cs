using Rubedo.Components;
using Rubedo.Graphics.Animation;
using Rubedo.Lib.StateMachine;
using Rubedo.Lib.StateMachine.Fluent;

namespace Rubedo.Lib.Extensions;

/// <summary>
/// Extensions for the state machine system, primarily for animation purposes.
/// </summary>
public static class StateMachineExtensions
{
    public static StateFluent<string, string> AnimateTexture(this StateFluent<string, string> state, Sprite sprite, AnimationInstance anim)
    {
        return state.Update(a =>
        {
            sprite.SetTexture(((SpriteAnimation)anim.Animation).Atlas.GetRegion(anim.CurrentFrame));
        });
    }
}