using Rubedo.Internal;
using Rubedo.Object;
using Rubedo.Rendering;
using System.Runtime.CompilerServices;

namespace Rubedo.Components;

public class Component : ITransformable, IDestroyable
{
    public bool IsDestroyed { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; set; } = false;

    public Entity Entity { get; protected set; }
    /// <summary>
    /// Short for <see cref="Entity.transform"/>
    /// </summary>
    public Transform Transform
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Entity.transform;
    }
    /// <summary>
    /// This component's local transform.
    /// </summary>
    public Transform compTransform;
    public bool active;
    public bool visible;

    public Component(bool active, bool visible)
    {
        compTransform = new Transform();
        compTransform.attached = this;
        this.active = active;
        this.visible = visible;
    }

    /// <summary>
    /// Called when this component is added to an Entity.
    /// </summary>
    public virtual void Added(Entity entity)
    {
        Entity = entity;
        compTransform.SetParent(entity.transform);
    }
    /// <summary>
    /// Called when this component is removed from an Entity.
    /// </summary>
    /// <param name="entity"></param>
    public virtual void Removed(Entity entity)
    {
        Entity = null;
        compTransform.SetParent(null);
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
    public virtual void EntityRemoved(GameState state) 
    {
        this.Entity = null;
    }

    /// <summary>
    /// Called every frame while <see cref="Entity.active"/> and <see cref="active"/> are both true.
    /// </summary>
    public virtual void Update() { }
    /// <summary>
    /// Called when this transform receives a change in a frame, I.E. the transform is marked "dirty".
    /// </summary>
    public virtual void TransformChanged() { }

    /// <summary>
    /// Draws the component, if needed.
    /// </summary>
    public virtual void Draw(Renderer sb)  { }

    /// <summary>
    /// 
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
        this.compTransform = null;
        IsDestroyed = true;
    }
}
