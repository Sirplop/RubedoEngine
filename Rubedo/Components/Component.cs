using Rubedo.Object;
using System;
using System.Runtime.CompilerServices;

namespace Rubedo.Components;

public abstract class Component : ITransformable, IDestroyable
{
    public bool IsDestroyed { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; set; } = false;

    /// <summary>
    /// The entity this component is attached to.
    /// </summary>
    public Entity Entity { get; protected set; }
    /// <summary>
    /// Gets the transform of the entity this component is attached to.
    /// </summary>
    public Transform Transform => Entity.Transform;

    public bool Active
    {
        get => Entity._active && _active;
        set
        {
            if (_active != value)
            {
                bool reallyActive = Active;
                _active = value;
                if (Active != reallyActive) //double check that the activation state actually changed.
                {
                    if (_active)
                        OnEnable();
                    else
                        OnDisable();
                }
            }
        }
    }
    internal bool _active = true;
    public bool Visible
    {
        get => Entity._visible && _visible;
        set
        {
            if (_visible != value)
                _visible = value;
        }
    }
    internal bool _visible = true;

    /// <summary>
    /// Called when this component is added to an Entity.
    /// </summary>
    public virtual void Added(Entity entity)
    {
        Entity = entity;
    }
    /// <summary>
    /// Called when this component is removed from an Entity.
    /// </summary>
    /// <param name="entity"></param>
    public virtual void Removed(Entity entity)
    {
        Entity = null;
    }

    /// <summary>
    /// Called when the Entity this component is attached to awakes.
    /// </summary>
    public virtual void EntityAwake() { }
    /// <summary>
    /// Called when the Entity this component is attached to is added to a scene.
    /// </summary>
    public virtual void EntityAdded(GameState state) { }
    /// <summary>
    /// Called when the Entity this component is attached to is removed from a scene.
    /// </summary>
    public virtual void EntityRemoved(GameState state) { }

    /// <summary>
    /// Called when this component is activated, or when its parent entity is activated.
    /// </summary>
    public virtual void OnEnable() { }

    /// <summary>
    /// Called when this component is deactivated, or when its parent entity is deactivated.
    /// </summary>
    public virtual void OnDisable() { }

    /// <summary>
    /// Called every frame while <see cref="Entity.active"/> and <see cref="active"/> are both true.
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// Called every fixed time step, dictated by <see cref="Time.FixedDeltaTime"/>.
    /// </summary>
    public virtual void FixedUpdate() { }
    /// <summary>
    /// Called when this component's transform receives a change in a frame, I.E. the transform is marked "dirty".
    /// </summary>
    public virtual void TransformChanged() { }

    /// <summary>
    /// Called when this component is destroyed.
    /// </summary>
    public virtual void OnDestroy() { }

    /// <summary>
    /// Removes this component from its parent entity. Short for calling <see cref="Entity.Remove(Component)"/>
    /// </summary>
    public void RemoveSelf()
    {
        if (Entity != null)
            Entity.Remove(this);
    }

    public void Destroy()
    {
        OnDestroy();
        IsDestroyed = true;
    }
}
