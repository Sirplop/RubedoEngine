using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Rubedo.Graphics;

/// <summary>
/// A texture + shader + blend-state combination that renderables draw with.
/// </summary>
public sealed class Material : IEquatable<Material>
{
    private readonly struct Key : IEquatable<Key>
    {
        public readonly Texture2D Texture;
        public readonly Effect Effect;
        public readonly BlendState BlendState;
        public readonly bool IsTransparent;

        public Key(Texture2D texture, Effect effect, BlendState blendState, bool isTransparent)
        {
            Texture = texture;
            Effect = effect;
            BlendState = blendState;
            IsTransparent = isTransparent;
        }

        public bool Equals(Key other)
        {
            return  Texture == other.Texture && 
                    Effect == other.Effect &&
                    BlendState == other.BlendState && 
                    IsTransparent == other.IsTransparent;
        }
        public override bool Equals(object obj)
        {
            return obj is Key k && Equals(k);
        }

        public override int GetHashCode() => HashCode.Combine(Texture, Effect, BlendState, IsTransparent);
    }

    private static readonly Dictionary<Key, Material> _cache = new Dictionary<Key, Material>();

    /// <summary>
    /// The texture this material draws with. Can be null (e.g. for effects that sample nothing, or aggregate draws like UI).
    /// </summary>
    public readonly Texture2D Texture;
    /// <summary>
    /// The shader this material draws with.
    /// </summary>
    public readonly Effect Effect;
    /// <summary>
    /// The blend state this material draws with.
    /// </summary>
    public readonly BlendState BlendState;

    /// <summary>
    /// Whether this material blends with what's already drawn (true, the default -
    /// <see cref="BlendState.AlphaBlend"/>) or overwrites it outright (false -
    /// <see cref="BlendState.Opaque"/>), when no explicit <c>blendState</c> is given.
    /// </summary>
    public readonly bool IsTransparent;

    /// <summary>
    /// True for Materials created through <see cref="Material.GetUnique(Texture2D, Effect, Action{Effect}, bool, BlendState)"/>
    /// </summary>
    private readonly bool _isUnique;

    private Material(Texture2D texture, Effect effect, BlendState blendState, bool isTransparent, bool isUnique)
    {
        Texture = texture;
        Effect = effect;
        BlendState = blendState;
        IsTransparent = isTransparent;
        _isUnique = isUnique;
    }

    /// <summary>
    /// Gets a shared material for the given texture/effect/blend/transparency
    /// combination. Using equivalent arguments returns the same instance.
    /// </summary>
    public static Material Get(Texture2D texture, Effect effect = null, bool isTransparent = true, BlendState blendState = null)
    {
        effect ??= RubedoEngine.Instance.Renderer.DefaultEffect;
        blendState ??= isTransparent ? BlendState.AlphaBlend : BlendState.Opaque;
        Key key = new Key(texture, effect, blendState, isTransparent);
        if (!_cache.TryGetValue(key, out Material mat))
        {
            mat = new Material(texture, effect, blendState, isTransparent, isUnique: false);
            _cache[key] = mat;
        }
        return mat;
    }

    /// <summary>
    /// Creates a brand-new, non-shared material with its own cloned <see cref="Effect"/>,
    /// for a renderable that needs shader parameter values no other renderable shares.
    /// </summary>
    public static Material GetUnique(Texture2D texture, Effect baseEffect, Action<Effect> configure, bool isTransparent = true, BlendState blendState = null)
    {
        Effect clone = (baseEffect ?? RubedoEngine.Instance.Renderer.DefaultEffect).Clone();
        configure?.Invoke(clone);
        blendState ??= isTransparent ? BlendState.AlphaBlend : BlendState.Opaque;
        return new Material(texture, clone, blendState, isTransparent, isUnique: true);
    }

    /// <summary>
    /// Releases the cloned effect backing a unique material. Do not call this on materials
    /// obtained through <see cref="Get"/>
    /// </summary>
    public void Dispose()
    {
        if (_isUnique)
            Effect?.Dispose();
    }

    public bool Equals(Material other) => ReferenceEquals(this, other);
    public override bool Equals(object obj) => Equals(obj as Material);
    public override int GetHashCode() => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
}