using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Rubedo.Object;
using Rubedo.Internal;
using Rubedo.UI;
using Rubedo.Rendering;
using Rubedo.Input;
using Microsoft.Xna.Framework;

namespace Rubedo;

/// <summary>
/// I am GameState, and this is my summary.
/// </summary>
public class GameState
{
    public readonly StateManager stateManager;

    protected string _name = "";

    protected internal EntityList Entities { get; private set; } //should probably get moved into the statemanager.
    protected internal RenderableComponentList Renderables { get; private set; }

    protected List<Camera> _cameras;
    /// <summary>
    /// The "primary" camera, useful if most things are getting drawn to one camera.
    /// </summary>
    public Camera MainCamera => mainCamera;
    protected Camera mainCamera = null;

    public string Name => _name;
    public GameState(StateManager sm)
    {
        Entities = new EntityList(this);
        Renderables = new RenderableComponentList();
        stateManager = sm;
        _cameras = new List<Camera>();
    }

    public void AddCamera(Camera camera, bool isMainCamera = false)
    {
        RubedoEngine.Instance.Window.ClientSizeChanged += camera.OnWindowSizeChanged;

        if (isMainCamera)
            mainCamera = camera;
        if (_cameras.Count == 0)
        {
            _cameras.Add(camera);
            mainCamera = camera; //by default, if there's only one camera, this is the main one.
            return;
        }
        for (int i = 0; i < _cameras.Count; i++)
        {
            if (_cameras[i].RenderOrder > camera.RenderOrder)
            {
                _cameras.Insert(i, camera);
                return;
            }
        }
        _cameras.Add(camera); //larger than all other camera orders
    }
    public void RemoveCamera(Camera camera)
    {
        for (int i = 0; i < _cameras.Count; i++)
        {
            if (_cameras[i] == camera)
            {
                _cameras.RemoveAt(i);
                RubedoEngine.Instance.Window.ClientSizeChanged -= camera.OnWindowSizeChanged;
                return;
            }
        }
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
        Renderables.Clear();
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
        for (int i = 0; i < _cameras.Count; i++)
        {
            DrawCamera(_cameras[i], sb);
        }
    }

    protected void DrawCamera(Camera camera, Renderer sb)
    {
        camera.SetViewport();
        for (int h = 0; h < camera.RenderLayers.Count; h++)
        {
            int layer = camera.RenderLayers[h];
            sb.Begin(camera, camera.samplerState);
            List<IRenderable> renderables = Renderables.ComponentsWithLayer(layer);
            for (int j = 0; j < renderables.Count; j++)
            {
                renderables[j].Render(sb, camera);
            }
            sb.End();
        }
        camera.ResetViewport();
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