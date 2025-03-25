using Rubedo.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Render;

namespace Rubedo.Components;

public class Component
{
    public Entity Entity { get; private set; }
    public Transform localTransform;
    public Transform worldTransform { get
        {
            return Entity.transform + localTransform;
        } 
    }
    public bool active;
    public bool visible;

    public Component(bool active, bool visible)
    {
        localTransform = new Transform();
        this.active = active;
        this.visible = visible;
    }

    public virtual void Added(Entity entity)
    {
        Entity = entity;
    }
    public virtual void Removed(Entity entity)
    {
        Entity = null;
    }

    /// <summary>
    /// Only called when the Entity this component is attached to awakes.
    /// </summary>
    public virtual void EntityAwake() { }
    public virtual void EntityAdded(GameState state) { }
    public virtual void EntityRemoved(GameState state) { }

    public virtual void Update() { }

    public virtual void Draw(Renderer sb)  { }

    public void RemoveSelf()
    {
        if (Entity != null)
            Entity.Remove(this);
    }
}
