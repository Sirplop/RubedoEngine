using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubedo.Graphics;

/// <summary>
/// Represents a texture composed of many <see cref="Texture2DRegion"/>.
/// </summary>
public class Texture2DAtlas : IEnumerable<Texture2DRegion>
{
    /// <summary>
    /// The backing texture of this atlas.
    /// </summary>
    public Texture2D Texture { get; }

    /// <summary>
    /// The name of this resource.
    /// </summary>
    public string Name { get; }

    private readonly List<Texture2DRegion> _regionsByIndex = new List<Texture2DRegion>();
    private readonly Dictionary<string, Texture2DRegion> _regionsByName = new Dictionary<string, Texture2DRegion>();

    public Texture2DAtlas(Texture2D texture, string name = "")
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
    public Texture2DRegion CreateRegion(int x, int y, int width, int height) => CreateRegion(new Rectangle(x, y, width, height), null);
    /// <summary>
    /// Constructs a new region, and adds it to the atlas.
    /// </summary>
    public Texture2DRegion CreateRegion(int x, int y, int width, int height, string name) => CreateRegion(new Rectangle(x, y, width, height), name);
    /// <summary>
    /// Constructs a new region, and adds it to the atlas.
    /// </summary>
    public Texture2DRegion CreateRegion(Rectangle bounds, string name = "")
    {
        Texture2DRegion region = new Texture2DRegion(Texture, bounds, name);
        AddRegion(region);
        return region;
    }

    private void AddRegion(Texture2DRegion region)
    {
        if (_regionsByName.ContainsKey(region.Name))
            throw new InvalidOperationException($"{Name} already contains a {nameof(Texture2DRegion)} with the name '{region.Name}'!");
        _regionsByIndex.Add(region);
        _regionsByName.Add(region.Name, region);
    }

    public bool RemoveRegion(string name)
    {
        if (TryGetRegion(name, out Texture2DRegion region))
        {
            return RemoveRegion(region);
        }

        return false;
    }
    public bool RemoveRegion(int index)
    {
        if (TryGetRegion(index, out Texture2DRegion region))
        {
            return RemoveRegion(region);
        }

        return false;
    }
    private bool RemoveRegion(Texture2DRegion region) => _regionsByIndex.Remove(region) && _regionsByName.Remove(region.Name);

    /// <summary>
    /// Gets the region at the specified index.
    /// </summary>
    public Texture2DRegion GetRegion(int index) => _regionsByIndex[index];

    /// <summary>
    /// Gets the region with the specified name.
    /// </summary>
    public Texture2DRegion GetRegion(string name) => _regionsByName[name];

    /// <summary>
    /// Tries to get the region at the specified index.
    /// </summary>
    public bool TryGetRegion(int index, out Texture2DRegion region)
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
    public bool TryGetRegion(string name, out Texture2DRegion region) => _regionsByName.TryGetValue(name, out region);

    /// <summary>
    /// Clears all regions from the atlas.
    /// </summary>
    public void ClearRegions()
    {
        _regionsByIndex.Clear();
        _regionsByName.Clear();
    }

    public IEnumerator<Texture2DRegion> GetEnumerator() => _regionsByIndex.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}