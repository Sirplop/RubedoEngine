using Rubedo.Components;
using Rubedo.Lib.StateMachine;
using System.Collections.Generic;

namespace Rubedo.Graphics.Animation;

/// <summary>
/// Manages an animation state machine, including automatic transitioning between animations.
/// </summary>
public class Animator : Component
{
    protected Fsm<string, string> machine;
    protected Dictionary<string, AnimationInstance> animationMap;
    protected AnimationInstance current;
    protected string currentName;

    /// <summary>
    /// Indicates whether this animation should abide by the time scale.
    /// </summary>
    public bool UseRealTime { get; set; }

    /// <summary>
    /// The currrent state name of the animator.
    /// </summary>
    public string CurrentName => currentName;
    /// <summary>
    /// The current animation playing through this animator.
    /// </summary>
    public AnimationInstance Current => current;

    /// <summary>
    /// Constructs the full animator.
    /// </summary>
    /// <param name="animations">The map of state names to animations.</param>
    /// <param name="animationMachine">The animation state machine.</param>
    public Animator(Dictionary<string, AnimationInstance> animations, Fsm<string, string> animationMachine)
    {
        SetNewAnimations(animations, animationMachine);
    }

    /// <summary>
    /// Replaces the current state machine and animation set with another one.
    /// </summary>
    /// <param name="animations">The map of state names to animations.</param>
    /// <param name="animationMachine">The animation state machine.</param>
    public void SetNewAnimations(Dictionary<string, AnimationInstance> animations, Fsm<string, string> animationMachine)
    {
        animationMap = animations;
        machine = animationMachine;
        currentName = machine.StartState.Identifier;
        current = animations[currentName];
        current.Play();

        machine.AddStateChangeHandler(ChangeStates);
    }

    private int startFrame = 0;
    /// <summary>
    /// Sets a trigger, which might cause a state change.
    /// </summary>
    /// <param name="trigger">The name of the trigger.</param>
    /// <param name="startFrame">If a state change is triggered, start the new animation at this frame.</param>
    /// <returns>If the trigger caused a state change.</returns>
    public bool Trigger(string trigger, int startFrame = 0)
    {
        if (machine == null)
            return false;

        this.startFrame = startFrame;
        State<string, string> curState = machine.Current;
        machine.Trigger(trigger);
        this.startFrame = 0;
        return curState != machine.Current;
    }

    /// <summary>
    /// Updates the animation when the state changes.
    /// </summary>
    protected void ChangeStates(object? sender, Lib.StateMachine.Events.StateChangeArgs<string, string> args)
    {
        if (sender == null)
            return; //dunno how this happened!
        current.Stop();
        currentName = args.To.Identifier;
        current = animationMap[currentName];
        current.Reset();
        current.Play(startFrame);
        startFrame = 0;
    }

    public override void Update()
    {
        base.Update();
        machine?.Update(UseRealTime ? Time.RawDeltaTimeMillis : Time.DeltaTimeMillis);
        current?.Update();
    }
}