using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubedo.Graphics.Sprites;

/// <summary>
/// Represents a texture composed of many <see cref="TextureRegion2D"/>.
/// </summary>
public class TextureAtlas2D : IEnumerable<TextureRegion2D>, IDisposable
{
    public bool Disposed { get; private set; }

    /// <summary>
    /// The backing texture of this atlas.
    /// </summary>
    public Texture2D Texture { get; }

    /// <summary>
    /// The name of this resource.
    /// </summary>
    public string Name { get; }

    private readonly List<TextureRegion2D> _regionsByIndex = new List<TextureRegion2D>();
    private readonly Dictionary<string, TextureRegion2D> _regionsByName = new Dictionary<string, TextureRegion2D>();

    public TextureAtlas2D(Texture2D texture, string name = "")
    {
        ArgumentNullException.ThrowIfNull(texture);
        if (texture.IsDisposed)
        {
            throw new ObjectDisposedException(nameof(texture), $"{nameof(texture)} was disposed prior");
        }

        if (string.IsNullOrEmpty(name))
            Name = $"{texture.Name}.Atlas";
        else
            Name = name;

        Texture = texture;
    }
    /// <summary>
    /// Constructs a new region, and adds it to the atlas.
    /// </summary>
    public TextureRegion2D CreateRegion(int x, int y, int width, int height) => CreateRegion(new Rectangle(x, y, width, height), null);
    /// <summary>
    /// Constructs a new region, and adds it to the atlas.
    /// </summary>
    public TextureRegion2D CreateRegion(int x, int y, int width, int height, string name) => CreateRegion(new Rectangle(x, y, width, height), name);
    /// <summary>
    /// Constructs a new region, and adds it to the atlas.
    /// </summary>
    public TextureRegion2D CreateRegion(Rectangle bounds, string name = "")
    {
        TextureRegion2D region = new TextureRegion2D(Texture, bounds, name);
        AddRegion(region);
        return region;
    }

    private void AddRegion(TextureRegion2D region)
    {
        if (_regionsByName.ContainsKey(region.Name))
            throw new InvalidOperationException($"{Name} already contains a {nameof(TextureRegion2D)} with the name '{region.Name}'!");
        _regionsByIndex.Add(region);
        _regionsByName.Add(region.Name, region);
    }

    public bool RemoveRegion(string name)
    {
        if (TryGetRegion(name, out TextureRegion2D region))
        {
            return RemoveRegion(region);
        }

        return false;
    }
    public bool RemoveRegion(int index)
    {
        if (TryGetRegion(index, out TextureRegion2D region))
        {
            return RemoveRegion(region);
        }

        return false;
    }
    private bool RemoveRegion(TextureRegion2D region) => _regionsByIndex.Remove(region) && _regionsByName.Remove(region.Name);

    /// <summary>
    /// Gets the region at the specified index.
    /// </summary>
    public TextureRegion2D GetRegion(int index) => _regionsByIndex[index];

    /// <summary>
    /// Gets the region with the specified name.
    /// </summary>
    public TextureRegion2D GetRegion(string name) => _regionsByName[name];

    /// <summary>
    /// Tries to get the region at the specified index.
    /// </summary>
    public bool TryGetRegion(int index, out TextureRegion2D region)
    {
        if (index < 0 || index >= _regionsByIndex.Count)
        {
            region = default;
            return false;
        }

        region = _regionsByIndex[index];
        return true;
    }

    /// <summary>
    /// Tries to get the region with the specified name.
    /// </summary>
    public bool TryGetRegion(string name, out TextureRegion2D region) => _regionsByName.TryGetValue(name, out region);

    /// <summary>
    /// Clears all regions from the atlas.
    /// </summary>
    public void ClearRegions()
    {
        _regionsByIndex.Clear();
        _regionsByName.Clear();
    }

    public IEnumerator<TextureRegion2D> GetEnumerator() => _regionsByIndex.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        if (Disposed)
            return;
        Disposed = true;
        Texture.Dispose();
        GC.SuppressFinalize(this);
    }
}