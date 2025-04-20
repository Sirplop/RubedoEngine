using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Rubedo.Object;
using Rubedo.Internal;
using Rubedo.Render;

namespace Rubedo;

/// <summary>
/// I am GameState, and this is my summary.
/// </summary>
public class GameState
{
    public readonly StateManager stateManager;
    public readonly InputManager inputManager;

    protected string _name = "";

    protected internal EntityList Entities { get; private set; }

    public string Name => _name;
    public GameState(StateManager sm, InputManager ih)
    {
        Entities = new EntityList(this);
        stateManager = sm;
        inputManager = ih;
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
            if (obj.visible)
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