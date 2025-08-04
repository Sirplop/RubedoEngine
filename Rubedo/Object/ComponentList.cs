using Rubedo.Components;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubedo.Object;

/// <summary>
/// I am ComponentList, and this is my summary.
/// </summary>
public class ComponentList : IEnumerable<Component>
{
    public enum LockStates { Open, Locked, NotAllowed };

    public Entity Entity { get; internal set; }

    private List<Component> components;
    private List<Component> toAdd;
    private List<Component> toRemove;

    private HashSet<Component> current;
    private HashSet<Component> adding;
    private HashSet<Component> removing;

    private LockStates lockState;
    internal LockStates LockState
    {
        get
        {
            return lockState;
        }

        set
        {
            lockState = value;

            if (toAdd.Count > 0)
            {
                foreach (var component in toAdd)
                {
                    if (!current.Contains(component))
                    {
                        current.Add(component);
                        components.Add(component);
                        component.Added(Entity);
                    }
                }

                adding.Clear();
                toAdd.Clear();
            }

            if (toRemove.Count > 0)
            {
                foreach (var component in toRemove)
                {
                    if (current.Contains(component))
                    {
                        current.Remove(component);
                        components.Remove(component);
                        component.Removed(Entity);
                    }
                }

                removing.Clear();
                toRemove.Clear();
            }
        }
    }

    internal ComponentList(Entity entity)
    {
        Entity = entity;

        components = new List<Component>();
        toAdd = new List<Component>();
        toRemove = new List<Component>();
        current = new HashSet<Component>();
        adding = new HashSet<Component>();
        removing = new HashSet<Component>();
    }

    public void Add(Component component)
    {
       switch (lockState)
        {
            case LockStates.Open:
                if (!current.Contains(component))
                {
                    current.Add(component);
                    components.Add(component);
                    component.Added(Entity);
                }
                break;
            case LockStates.Locked:
                if (!current.Contains(component) && !adding.Contains(component))
                {
                    adding.Add(component);
                    toAdd.Add(component);
                }
                break;
            case LockStates.NotAllowed:
                throw new Exception("[ComponentList] Cannot add new Components during draw!");
        }
    }

    public void Remove(Component component)
    {
        switch (lockState)
        {
            case LockStates.Open:
                if (current.Contains(component))
                {
                    current.Remove(component);
                    components.Remove(component);
                    component.Removed(Entity);
                }
                break;

            case LockStates.Locked:
                if (current.Contains(component) && !removing.Contains(component))
                {
                    removing.Add(component);
                    toRemove.Add(component);
                }
                break;
            case LockStates.NotAllowed:
                throw new Exception("[ComponentList] Cannot remove Components during draw!");
        }
    }

    public void Add(IEnumerable<Component> components)
    {
        foreach (var component in components)
            Add(component);
    }

    public void Remove(IEnumerable<Component> components)
    {
        foreach (var component in components)
            Remove(component);
    }

    public void Add(params Component[] components)
    {
        foreach (var component in components)
            Add(component);
    }

    public void Remove(params Component[] components)
    {
        foreach (var component in components)
            Remove(component);
    }
    internal void Update()
    {
        LockState = LockStates.Locked;
        foreach (var component in components)
            if (component._active)
                component.Update();
        LockState = LockStates.Open;
    }

    internal void FixedUpdate()
    {
        LockState = LockStates.Locked;
        foreach (var component in components)
            if (component._active)
                component.FixedUpdate();
        LockState = LockStates.Open;
    }
    internal void TransformChanged()
    {
        LockState = LockStates.Locked;
        foreach (var component in components)
            component.TransformChanged();
        LockState = LockStates.Open;
    }

    public IEnumerator<Component> GetEnumerator()
    {
        return current.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return current.GetEnumerator();
    }
}