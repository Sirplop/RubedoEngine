using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubedo.Resources.AssetBox;

/// <summary>
/// A strongly-referenced asset type that will never be automatically garbage collected.
/// Use for things that should always be loaded, like fonts.
/// </summary>
public abstract class StrongAssetBox<T> : IAssetBox, IEnumerable<KeyValuePair<string, T>> where T : class
{
    public bool IsWeakReference => false;
    public string RootPath { get; protected set; }
    public Type Type { get; protected set; }

    public readonly string Name;
    public int Count => _assets.Count;


    private Dictionary<string, T> _assets = new Dictionary<string, T>();

    public StrongAssetBox(string name, string rootPath)
    {
        Type = typeof(T);
        if (name == string.Empty)
        {
            name = Type.Name;
        }
        else
        {
            Name = name;
        }
        RootPath = rootPath;
        Assets.RegisterAssetBox(name, this);
    }

    public abstract void LoadAsset(string key, string[] extra = null);
    public abstract void UnloadAsset(string key);

    /// <summary>
    /// Returns the asset with a specific key.
    /// </summary>
    public T GetAsset(string key) => _assets[key];

    /// <summary>
    /// Returns the asset with a specific key.
    /// </summary>
    public bool TryGetAsset(string key, out T value) => _assets.TryGetValue(key, out value);

    /// <summary>
    /// Returns true, if the box contains asset with provided key.
    /// </summary>
    public bool ContainsAsset(string key) => _assets.ContainsKey(key);

    public void AddAsset(string key, T asset) => _assets.Add(key, asset);

    public void RemoveAsset(string key) => _assets.Remove(key);

    /// <summary>
    /// Removes all assets from the box.
    /// </summary>
    public void Clear() => _assets.Clear();

    public List<string> GetAssets()
    {
        List<string> assets = new List<string>();
        assets.AddRange(_assets.Keys);
        return assets;
    }

    IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, T>>)_assets).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, T>>)_assets).GetEnumerator();
    }
}