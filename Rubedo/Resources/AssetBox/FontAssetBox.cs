using FontStashSharp;
using Microsoft.Xna.Framework;
using System.IO;
using System;
using Microsoft.Xna.Framework.Input;

namespace Rubedo.Resources.AssetBox;

/// <summary>
/// TODO: I am FontAssetType, and I don't have a summary yet.
/// </summary>
public class FontAssetBox : StrongAssetBox<FontSystem>
{
    public FontAssetBox(string name, string rootPath) : base(name, rootPath) { }

    public override void LoadAsset(string fontName, string[] fontPaths)
    {
        if (fontName == string.Empty)
            return;

        if (ContainsAsset(fontName))
            return;

        string root = Path.Combine(Assets.RootDirectory, RootPath);
        FontSystem fontSystem = new FontSystem();
        for (int i = 0; i < fontPaths.Length; i++)
        {
            try
            {
                string path = Path.Combine(root, fontPaths[i]);
                Stream stream = TitleContainer.OpenStream(path);
                fontSystem.AddFont(stream);
            }
            catch
            {
                throw; //if the font file doesn't exist, this will explode. Which is good.
            }
        }
        AddAsset(fontName, fontSystem);
    }

    public override void UnloadAsset(string key)
    {
        if (ContainsAsset(key))
        {
            FontSystem fontSystem = GetAsset(key);
            fontSystem?.Dispose();
        }
        RemoveAsset(key);
    }
}