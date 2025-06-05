using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Rubedo.Object;
using Rubedo.Internal;
using Rubedo.UI;
using Rubedo.Rendering;
using Rubedo.Input;

namespace Rubedo;

/// <summary>
/// I am GameState, and this is my summary.
/// </summary>
public class GameState
{
    public readonly StateManager stateManager;

    protected string _name = "";

    protected internal EntityList Entities { get; private set; } //should probably get moved into the statemanager.

    public string Name => _name;
    public GameState(StateManager sm)
    {
        Entities = new EntityList(this);
        stateManager = sm;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
        foreach (Entity obj in Entities)
        {
            obj.Removed(this);
            Entities.Remove(obj);
        }
        Entities.UpdateLists();
        GUI.Root.DestroyChildren();
    }

    public virtual void LoadContent()
    {
    }

    public virtual void HandleInput()
    {
    }

    public virtual void Update()
    {
        HandleInput();
        Entities.UpdateLists();
        Entities.Update();
    }

    public virtual void Draw(Renderer sb)
    {
        foreach (Entity obj in Entities)
        {
            if (obj._visible)
                obj.Draw(sb);
        }
    }

    #region Object adding
    public void Add(Entity obj)
    {
        Entities.Add(obj);
    }

    public void Remove(Entity obj)
    {
        Entities.Remove(obj);
    }

    public void Add(IEnumerable<Entity> objs)
    {
        Entities.Add(objs);
    }

    public void Remove(IEnumerable<Entity> entities)
    {
        Entities.Remove(entities);
    }

    public void Add(params Entity[] entities)
    {
        Entities.Add(entities);
    }

    public void Remove(params Entity[] entities)
    {
        Entities.Remove(entities);
    }
    #endregion
}