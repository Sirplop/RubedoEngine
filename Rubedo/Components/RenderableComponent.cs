using Microsoft.Xna.Framework;
using Rubedo.Graphics;
using Rubedo.Lib;
using Rubedo.Object;
using System;

namespace Rubedo.Components;

/// <summary>
/// TODO: I am RenderableComponent, and I don't have a summary yet.
/// </summary>
public abstract class RenderableComponent : Component, IRenderable
{
    internal const float LAYER_SCALE = 1.175494e-6f; //Very small float value that's still large enough to do math with 0.5.

    protected GameState attachedState = null;

    public abstract RectF Bounds { get; }

    public int LayerDepth 
    {
        get => _realLayerDepth;
        set
        {
            _layerDepth = (value * LAYER_SCALE) - 5e-1f;
            _realLayerDepth = value;
        }
    }
    protected int _realLayerDepth = 0;
    protected float _layerDepth = 0;

    public int RenderLayer 
    { 
        get => _renderLayer; 
        set
        {
            if (_renderLayer != value)
            {
                attachedState?.Renderables.UpdateRenderableLayer(this, _renderLayer, value);
                _renderLayer = value;
            }
        }
    }
    protected int _renderLayer = (int)Graphics.Sprites.RenderLayer.Default;

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