using Rubedo.Graphics.Sprites;
using Rubedo.Resources.Serializers;
using System;
using System.IO;

namespace Rubedo.Resources.AssetBox;

/// <summary>
/// TODO: I am TextureAtlasAssetType, and I don't have a summary yet.
/// </summary>
public class TextureAtlasAssetBox : WeakAssetBox<TextureAtlas2D>
{
    public TextureAtlasAssetBox(string name, string rootPath) : base(name, rootPath) { }

    public override void LoadAsset(string key, string[] extra = null)
    {
        if (key == string.Empty)
            return;

        if (AssetExists(key))
            return;

        string map = Path.Combine(Assets.RootDirectory, RootPath, key);

        TextureAtlas2D atlas = TextureAtlas2DLoader.Load(map);
        AddAsset(key, atlas);
    }

    public override void UnloadAsset(string key)
    {
        throw new NotImplementedException();
    }
}