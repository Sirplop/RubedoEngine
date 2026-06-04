using System;

namespace Rubedo.Resources;

/// <summary>
/// TODO: I am IAssetType, and I don't have a summary yet.
/// </summary>
public interface IAssetBox
{
    public bool IsWeakReference { get; }
    Type Type { get; }
    public string RootPath { get; }

    /// <summary>
    /// Load an asset at the given path, relative to the DefaultPath.
    /// </summary>
    void LoadAsset(string key, string[] extra = null);
    /// <summary>
    /// Unload an asset at the given path from this AssetType.
    /// </summary>
    void UnloadAsset(string key);
}