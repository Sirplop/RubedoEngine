using Rubedo.Components;
using Rubedo.Internal;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using Rubedo.Rendering;
using System.Runtime.CompilerServices;

namespace Rubedo.Object;

public class Entity : IEnumerable<Component>, IEnumerable, ITransformable, IDestroyable
{
    public GameState State {  get; private set; }
    public ComponentList Components { get; private set; }
    public bool IsDestroyed { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; set; } = false;

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

    public void Added(GameState state) 
    {
        State = state;
        if (Components != null)
            foreach (var c in Components)
                c.EntityAdded(state);
    }
    public void Awake(GameState state)
    {
        if (Components != null)
            foreach (var c in Components)
                c.EntityAwake();
    }
    public void Removed(GameState state)
    {
        if (Components != null)
            foreach (var c in Components)
                c.EntityRemoved(state);
        State = null;
    }

    public void Update()
    {
        Components.Update();
    }
    void ITransformable.TransformChanged()
    {
        Components.TransformChanged();
    }
    public void Draw(Renderer sb)
    {
        Components.Draw(sb);
    }

    public void Add(Component component)
    {
        Components.Add(component);
    }
    public void Remove(Component component)
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

    /// <summary>
    /// Destroys this entity and all components attached to it.
    /// </summary>
    public void Destroy()
    {
        if (IsDestroyed)
            return;
        foreach (Component c in Components)
        {
            c.Destroy();
        }
        this.Components = null;
        this.transform = null;
        State.Remove(this);
        IsDestroyed = true;
    }
}