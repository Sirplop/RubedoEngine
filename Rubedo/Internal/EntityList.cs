using Rubedo.Object;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Rubedo.Internal;

/// <summary>
/// I am GameObjectList, and this is my summary.
/// </summary>
public class EntityList : IEnumerable<Entity>
{
    public GameState State { get; private set; }

    private List<Entity> entities;
    private List<Entity> toAdd;
    private List<Entity> toAwake;
    private List<Entity> toRemove;

    private HashSet<Entity> current;
    private HashSet<Entity> adding;
    private HashSet<Entity> removing;

    public int Count => current.Count;

    internal EntityList(GameState state)
    {
        State = state;

        entities = new List<Entity>();
        toAdd = new List<Entity>();
        toAwake = new List<Entity>();
        toRemove = new List<Entity>();

        current = new HashSet<Entity>();
        adding = new HashSet<Entity>();
        removing = new HashSet<Entity>();
    }

    public void UpdateLists()
    {
        if (toAdd.Count > 0)
        {
            for (int i = 0; i < toAdd.Count; i++)
            {
                var entity = toAdd[i];
                if (current.Add(entity))
                {
                    entities.Add(entity);

                    if (State != null)
                    {
                        entity.Added(State);
                    }
                }
            }
        }

        if (toRemove.Count > 0)
        {
            for (int i = 0; i < toRemove.Count; i++)
            {
                var entity = toRemove[i];
                if (current.Remove(entity))
                {
                    entities.Remove(entity);

                    if (State != null)
                    {
                        entity.Removed(State);
                    }
                }
            }

            toRemove.Clear();
            removing.Clear();
        }

        if (toAdd.Count > 0)
        {
            toAwake.AddRange(toAdd);
            toAdd.Clear();
            adding.Clear();

            for (int i = 0; i < toAwake.Count; i++)
            {
                var entity = toAwake[i];
                if (entity.State == State)
                    entity.Awake(State);
            }
            toAwake.Clear();
        }
    }
    internal void Update()
    {
        foreach (var entity in entities)
            if (entity._active)
                entity.Update();
    }

    #region Add/Remove
    public void Add(Entity entity)
    {
        if (!adding.Contains(entity) && !current.Contains(entity))
        {
            adding.Add(entity);
            toAdd.Add(entity);
        }
    }

    public void Remove(Entity entity)
    {
        if (!removing.Contains(entity) && current.Contains(entity))
        {
            removing.Add(entity);
            toRemove.Add(entity);
        }
    }
    public void Add(params Entity[] entities)
    {
        for (int i = 0; i < entities.Length; i++)
            Add(entities[i]);
    }

    public void Remove(params Entity[] entities)
    {
        for (int i = 0; i < entities.Length; i++)
            Remove(entities[i]);
    }

    public void Add(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Add(entity);
    }

    public void Remove(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Remove(entity);
    }
    #endregion

    public IEnumerator<Entity> GetEnumerator()
    {
        return current.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return current.GetEnumerator();
    }
}