using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Rendering;

namespace Rubedo.Components;

/// <summary>
/// TODO: I am RenderableComponent, and I don't have a summary yet.
/// </summary>
public abstract class RenderableComponent : Component, IRenderable
{
    protected GameState attachedState;

    public abstract RectF Bounds { get; }

    public float LayerDepth { get => _layerDepth; set => _layerDepth = value; }
    protected float _layerDepth = 0;
    public int RenderLayer { get => _renderLayer; set => _renderLayer = value; }

    protected int _renderLayer = (int)Rendering.RenderLayer.Default;

    public virtual bool IsVisibleToCamera(Camera camera)
    {
        return camera.Intersects(Bounds);
    }

    public abstract void Render(Renderer renderer, Camera camera);

    public override void EntityAdded(GameState state)
    {
        state.Renderables.Add(this);
        attachedState = state;
    }

    public override void Added(Entity entity)
    {
        base.Added(entity);
        if (entity.State != null)
        { //entity already spawned, put this into the renderables list.
            entity.State.Renderables.Add(this);
            attachedState = entity.State;
        }
    }

    public override void EntityRemoved(GameState state)
    {
        if (state == attachedState)
        {
            state.Renderables.Remove(this);
            attachedState = null;
        }
    }

    public override void OnDestroy()
    {
        if (attachedState != null)
        {
            attachedState.Renderables.Remove(this);
            attachedState = null;
        }
    }
}