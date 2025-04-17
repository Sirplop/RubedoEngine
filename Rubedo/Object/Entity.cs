using Rubedo.Components;
using Rubedo.Internal;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using Rubedo.Render;

namespace Rubedo.Object;

public class Entity : IEnumerable<Component>, IEnumerable, ITransformable
{
    public GameState State {  get; private set; }
    public ComponentList Components { get; private set; }
    public bool active = true;
    public bool visible = true;
    public Transform transform;
    public Entity() : this(Vector2.Zero, 0, Vector2.One) { }
    public Entity(Vector2 position) : this(position, 0, Vector2.One) { }
    public Entity(Vector2 position, float rotation) : this(position, rotation, Vector2.One) { }
    public Entity(Vector2 position, float rotation, Vector2 scale)
    {
        transform = new Transform(position, rotation, scale);
        transform.attached = this;
        Components = new ComponentList(this);
    }

    public virtual void Added(GameState state) 
    {
        State = state;
    }
    public virtual void Awake(GameState state) { }
    public virtual void Removed(GameState state) 
    {
        if (Components != null)
            foreach (var c in Components)
                c.EntityRemoved(state);
        State = null;
    }

    public virtual void Update()
    {
        if (active)
            Components.Update();
    }
    void ITransformable.TransformChanged()
    {
        Components.TransformChanged();
    }
    public virtual void Draw(Renderer sb)
    {
        if (visible)
            Components.Draw(sb);
    }

    public virtual void Add(Component component)
    {
        Components.Add(component);
    }
    public virtual void Remove(Component component)
    {
        Components.Remove(component);
    }

    public IEnumerator<Component> GetEnumerator()
    {
        return Components.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Components.GetEnumerator();
    }
}