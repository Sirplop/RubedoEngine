using Microsoft.Xna.Framework.Graphics;
using Rubedo.Graphics;
using Rubedo.Lib;
using Rubedo.Object;

namespace Rubedo.Components;

/// <summary>
/// TODO: I am RenderableComponent, and I don't have a summary yet.
/// </summary>
public abstract class RenderableComponent : Component, IRenderable
{
    internal const float LAYER_SCALE = 1.175494e-6f; //Very small float value that's still large enough to do math with 0.5.

    protected GameState attachedState = null;

    public abstract RectF Bounds { get; }
    public bool AlwaysDraw { get; set; }

    public int LayerDepth 
    {
        get => _realLayerDepth;
        set
        {
            _layerDepth = (value * LAYER_SCALE) - 5e-1f;
            _realLayerDepth = value;
            attachedState?.Renderables.MarkDirty(_renderLayer);
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

    /// <summary>
    /// An optional shader override for this renderable. Null uses the renderer's default effect.
    /// </summary>
    public Effect Shader { get; set; } = null;

    /// <summary>
    /// See <see cref="Material.IsTransparent"/>. Picks this renderable's default BlendState -
    /// <seealso cref="BlendState.AlphaBlend"/> when true, <seealso cref="BlendState.Opaque"/> when false.
    /// </summary>
    public bool IsTransparent { get; set; } = true;

    /// <summary>
    /// The texture backing this renderable's Material.
    /// </summary>
    protected abstract Texture2D MaterialTexture { get; }

    private Material _materialCache;
    private Texture2D _cachedTexture;
    private Effect _cachedEffect;
    private bool _cachedTransparent;

    public Material GetMaterial()
    {
        Texture2D texture = MaterialTexture;
        if (_materialCache == null || texture != _cachedTexture || Shader != _cachedEffect || IsTransparent != _cachedTransparent)
        {
            _materialCache = Material.Get(texture, Shader, IsTransparent);
            _cachedTexture = texture;
            _cachedEffect = Shader;
            _cachedTransparent = IsTransparent;
        }
        return _materialCache;
    }

    public virtual bool IsVisibleToCamera(Camera camera)
    {
        return AlwaysDraw || camera.Intersects(Bounds);
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