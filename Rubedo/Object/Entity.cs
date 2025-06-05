using Rubedo.Components;
using Rubedo.Internal;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using Rubedo.Rendering;
using System.Runtime.CompilerServices;

namespace Rubedo.Object;

public sealed class Entity : IEnumerable<Component>, IEnumerable, ITransformable, IDestroyable
{
    public GameState State {  get; private set; }
    public ComponentList Components { get; private set; }
    public bool IsDestroyed { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; set; } = false;

    public bool Active
    {
        get => _active;
        set
        {
            if (_active != value)
            {
                _active = value;
                if (_active)
                    OnEnable();
                else
                    OnDisable();
            }
        }
    }
    internal bool _active = true;
    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible != value)
                _visible = value;
        }
    }
    internal bool _visible = true;

    public Transform transform;

    internal bool _hasAwakened = false;

    public Entity() : this(Vector2.Zero, 0, Vector2.One) { }
    public Entity(Vector2 position) : this(position, 0, Vector2.One) { }
    public Entity(Vector2 position, float rotation) : this(position, rotation, Vector2.One) { }
    public Entity(Vector2 position, float rotation, Vector2 scale)
    {
        transform = new Transform(position, rotation, scale);
        transform.attached = this;
        Components = new ComponentList(this);
    }

    internal void Added(GameState state) 
    {
        State = state;
        if (Components != null)
            foreach (var c in Components)
                c.EntityAdded(state);
    }
    internal void Awake(GameState state)
    {
        if (_hasAwakened)
            return;
        _hasAwakened = true;
        if (Components != null)
            foreach (var c in Components)
                c.EntityAwake();
    }

    internal void OnEnable()
    {
        if (Components != null)
            foreach (var c in Components)
                c.OnEnable();
    }

    internal void OnDisable()
    {
        if (Components != null)
            foreach (var c in Components)
                c.OnDisable();
    }

    internal void Removed(GameState state)
    {
        if (Components != null)
            foreach (var c in Components)
                c.EntityRemoved(state);
        State = null;
    }

    internal void Update()
    {
        if (_active)
            Components.Update();
    }
    void ITransformable.TransformChanged()
    {
        Components.TransformChanged();
    }
    internal void Draw(Renderer sb)
    {
        if (_visible)
            Components.Draw(sb);
    }

    public Entity Add(Component component)
    {
        Components.Add(component);
        return this;
    }
    public Entity Remove(Component component)
    {
        Components.Remove(component);
        return this;
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