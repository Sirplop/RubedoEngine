using Rubedo.Graphics;
using System.Collections.Generic;

namespace Rubedo.Object;

/// <summary>
/// The list of renderable components that are to be rendered by the game state.
/// </summary>
public class RenderableComponentList
{
    //NOTE: In the future, there might need to be a layer sorter function if we implement our own batcher. For now, it's unnecessary. I think.

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
        if (!list.Contains(component))
            list.Add(component);
        else
            RubedoEngine.Logger.Error("Renderable layer list aleady contains this component!");
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