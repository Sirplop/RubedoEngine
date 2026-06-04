using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;

namespace Rubedo.Resources.AssetBox;

/// <summary>
/// TODO: I am TextureAssetType, and I don't have a summary yet.
/// </summary>
public class TextureAssetBox : WeakAssetBox<Texture2D>
{
    private static readonly string[] supportedTexture2DExtensions = new string[4] { ".png", ".jpg", ".jpeg", ".bmp" };

    public TextureAssetBox(string name, string rootPath) : base(name, rootPath) { }

    public override void LoadAsset(string key, string[] extra = null)
    {
        if (key == string.Empty)
            return;

        if (AssetExists(key)) 
            return;

        string path = Path.Combine(Assets.RootDirectory, RootPath, key);
        foreach (string extension in supportedTexture2DExtensions)
        {
            try
            {
                using (Stream stream = TitleContainer.OpenStream(Path.ChangeExtension(path, extension)))
                {
                    if (stream != null)
                    {
                        Texture2D texture = Texture2D.FromStream(RubedoEngine.Graphics.GraphicsDevice, stream, DefaultColorProcessors.PremultiplyAlpha);
                        AddAsset(key, texture);
                        return;
                    }
                }
            }
            catch { } //TODO: Make a better way to handle this whole thing.
        }
        throw new ContentLoadException($"Texture at '{path}' does not exist!");
    }

    public override void UnloadAsset(string key)
    {
        Texture2D texture = GetAsset(key);
        if (texture != null)
        {
            texture.Dispose();
        }
        RemoveAsset(key);
    }
}