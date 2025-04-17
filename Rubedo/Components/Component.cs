using Rubedo.Object;
using Rubedo.Render;

namespace Rubedo.Components;

public class Component : ITransformable
{
    public Entity Entity { get; protected set; }
    public Transform transform;
    public bool active;
    public bool visible;

    public Component(bool active, bool visible)
    {
        transform = new Transform();
        transform.attached = this;
        this.active = active;
        this.visible = visible;
    }

    public virtual void Added(Entity entity)
    {
        Entity = entity;
        transform.SetParent(entity.transform);
    }
    public virtual void Removed(Entity entity)
    {
        Entity = null;
        transform.SetParent(null);
    }

    /// <summary>
    /// Only called when the Entity this component is attached to awakes.
    /// </summary>
    public virtual void EntityAwake() { }
    public virtual void EntityAdded(GameState state) { }
    public virtual void EntityRemoved(GameState state) 
    {
        this.Entity = null;
    }

    public virtual void Update() { }
    public virtual void TransformChanged() { }

    public virtual void Draw(Renderer sb)  { }

    public void RemoveSelf()
    {
        if (Entity != null)
            Entity.Remove(this);
    }
}
