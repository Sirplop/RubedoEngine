using Rubedo.Graphics;
using System.Collections;
using System.Collections.Generic;

namespace Rubedo.Object;

/// <summary>
/// The list of renderable components that are to be rendered by the game state.
/// </summary>
public class RenderableComponentList
{
    /// <summary>
    /// Renderables are sorted into rendering layers, for easy drawing.
    /// </summary>
    private readonly Dictionary<int, List<IRenderable>> _renderablesByLayer = new Dictionary<int, List<IRenderable>>();

    public void Add(IRenderable component)
    {
        AddToRenderLayer(component, component.RenderLayer);
    }
    public void Remove(IRenderable component)
    {
        _renderablesByLayer[component.RenderLayer].Remove(component);
    }

    public void UpdateRenderableLayer(IRenderable component, int oldLayer, int newLayer)
    {
        if (_renderablesByLayer.TryGetValue(oldLayer, out List<IRenderable> value))
        {
            value.Remove(component);
            AddToRenderLayer(component, newLayer);
        }
    }

    private void AddToRenderLayer(IRenderable component, int layer)
    {
        List<IRenderable> list = ComponentsWithLayer(layer);
        int index = FindSortIndex(component, list);
        list.Insert(index, component);
    }

    private int FindSortIndex(IRenderable component, List<IRenderable> list)
    {
        int high = list.Count - 1;
        int low = 0;
        int mid = 0;
        if (list.Count == 0 || list[0].LayerDepth >= component.LayerDepth)
            return 0;
        else if (list[high].LayerDepth <= component.LayerDepth)
            return high;
        else
        {
            while (low <= high)
            {
                mid = (high + low) / 2;
                if (list[mid].LayerDepth >= component.LayerDepth)
                    high = mid - 1;
                else
                    low = mid + 1;
            }
            return mid;
        }
    }

    public List<IRenderable> ComponentsWithLayer(int layer)
    {
        if (!_renderablesByLayer.TryGetValue(layer, out List<IRenderable> value))
        {
            value = _renderablesByLayer[layer] = new List<IRenderable>();
        }
        return value;
    }

    public void Clear()
    {
        _renderablesByLayer.Clear();
    }
}