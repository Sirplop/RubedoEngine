using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubedo.Resources;

/// <summary>
/// A weakly referenced asset type that will be garbage collected if not in use.
/// Use for ephemeral things, like textures and sounds.
/// </summary>
public abstract class WeakAssetBox<T> : IAssetBox, IEnumerable<KeyValuePair<string, T>> where T : class
{
    public bool IsWeakReference => true;
    public string RootPath { get; protected set; }
    public Type Type { get; protected set; }

    public readonly string Name;
    public int Count => _assets.Count;


    private readonly Dictionary<string, WeakReference<T>> _assets = new Dictionary<string, WeakReference<T>>();

    public WeakAssetBox(string name, string rootPath)
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
    public T GetAsset(string key)
    {
        T value = null;
        if (_assets.TryGetValue(key, out WeakReference<T> asset))
        {
            asset.TryGetTarget(out value);
        }
        
        if (value == null) //doesn't exist or got collected, load it again!
        {
            LoadAsset(key);
            if (_assets.TryGetValue(key, out asset))
            {
                asset.TryGetTarget(out value);
            }
        }
        return value;
    }

    /// <summary>
    /// Returns the asset with a specific key.
    /// </summary>
    public bool TryGetAsset(string key, out T value)
    {
        value = null;
        if (_assets.ContainsKey(key))
        {
            value = GetAsset(key);
            return value == null;
        }
        return false;
    }

    /// <summary>
    /// Returns true, if the box contains asset with provided key.
    /// </summary>
    protected bool ContainsAsset(string key) => _assets.ContainsKey(key);

    protected bool AssetExists(string key)
    {
        return _assets.TryGetValue(key, out WeakReference<T> value) && value.TryGetTarget(out _);
    }

    protected void AddAsset(string key, T asset)
    {
        if (ContainsAsset(key))
        {
            _assets[key].SetTarget(asset);
        }
        else
        {
            _assets.Add(key, new WeakReference<T>(asset));
        }
    }

    protected void RemoveAsset(string key) => _assets.Remove(key);

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