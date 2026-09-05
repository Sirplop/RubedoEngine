using Rubedo.Graphics;
using Rubedo.Lib.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rubedo.Object;

/// <summary>
/// The list of renderable components that are to be rendered by the game state.
/// </summary>
public class RenderableComponentList : IComparer<IRenderable>
{
    /// <summary>
    /// Renderables are sorted into rendering layers, for easy drawing.
    /// </summary>
    private readonly Dictionary<int, List<IRenderable>> _layers = new Dictionary<int, List<IRenderable>>();
    private readonly Dictionary<int, bool> _dirtyLayers = new Dictionary<int, bool>();

    private static readonly List<IRenderable> EmptyLayer = new List<IRenderable>();

    public void Add(IRenderable component)
    {
        AddToLayer(component, component.RenderLayer);
        MarkDirty(component.RenderLayer);
    }
    public void Remove(IRenderable component)
    {
        RemoveFromLayer(component, component.RenderLayer);
        MarkDirty(component.RenderLayer);
    }

    public void UpdateRenderableLayer(IRenderable component, int oldLayer, int newLayer)
    {
        RemoveFromLayer(component, oldLayer);
        AddToLayer(component, newLayer);
        MarkDirty(component.RenderLayer);
    }

    private void AddToLayer(IRenderable component, int layer)
    {
        GetOrCreateLayer(layer).Add(component);
    }

    private void RemoveFromLayer(IRenderable component, int layer)
    {
        if (_layers.TryGetValue(layer, out List<IRenderable> list))
        {
            int index = list.IndexOf(component);
            if (index < 0)
                return;
            list.SwapAndRemove(index);
            _dirtyLayers.AddOrSet(layer, true);
        }
    }
    private List<IRenderable> GetOrCreateLayer(int layer)
    {
        if (!_layers.TryGetValue(layer, out List<IRenderable> list))
        {
            list = new List<IRenderable>();
            _layers[layer] = list;
        }
        return list;
    }

    public void SortLayers()
    {
        foreach (int layer in _layers.Keys)
        {
            if (_dirtyLayers.ContainsKey(layer))
            {
                _layers[layer].Sort(this);
            }
        }
        _dirtyLayers.Clear();
    }

    /// <summary>
    /// Gets the renderables of a given layer. Make sure to call <see cref="SortLayers"/> beforehand!
    /// </summary>
    public List<IRenderable> GetLayer(int layer)
    {
        return _layers.TryGetValue(layer, out List<IRenderable> list) ? list : EmptyLayer;
    }

    public void Clear()
    {
        _layers.Clear();
        _dirtyLayers.Clear();
    }

    public int Compare(IRenderable a, IRenderable b)
    {
        int depth = a.LayerDepth.CompareTo(b.LayerDepth);
        if (depth != 0)
            return depth;
        return RuntimeHelpers.GetHashCode(a).CompareTo(RuntimeHelpers.GetHashCode(b));
    }

    public void MarkDirty(int layer)
    {
        _dirtyLayers.AddOrSet(layer, true);
    }
}