using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rubedo.Internal.Assets;

/// <summary>
/// I am AssetManager, and this is my summary.
/// </summary>
public static class AssetManager
{
    /// <summary>
    /// The directory path appended to the root directory for textures.
    /// </summary>
    public static string TexturePath = "textures";

    private static string _rootDirectory = string.Empty;
    /// <summary>
    /// The root directory (relative to <see cref="TitleContainer"/>) that assets will be searched for in.
    /// </summary>
    public static string RootDirectory
    {
        get
        {
            return _rootDirectory;
        }
        set
        {
            _rootDirectory = value;
        }
    }
    private static readonly string[] supportedTexture2DExtensions = new string[4] { ".png", ".jpg", ".jpeg", ".bmp" };

    private static Dictionary<string, FontSystem> loadedFonts;
    private static Dictionary<string, WeakReference<Texture2D>> loadedTextures;


    public static void Initialize(string rootDirectory)
    {
        RootDirectory = rootDirectory;
        loadedFonts = new Dictionary<string, FontSystem>();
        loadedTextures = new Dictionary<string, WeakReference<Texture2D>>();
        //TODO: add missing asset things
    }
    /// <summary>
    /// Create a new font system with the given lowercase <paramref name="fontName"/>, which includes the given <paramref name="fontPaths"/>.
    /// </summary>
    /// <returns>The new font system, or the existing one if it already exists.</returns>
    /// <remarks>The font paths are relative to the content root path.</remarks>
    public static FontSystem CreateNewFontSystem(string fontName, params string[] fontPaths)
    {
        if (loadedFonts == null)
            throw new NullReferenceException("Trying to access fonts before assets have been loaded. Don't do that!");

        fontName = fontName.ToLower();
        if (loadedFonts.TryGetValue(fontName, out FontSystem font))
        {
            return font;
        }

        FontSystem fontSystem = new FontSystem();
        for (int i = 0; i < fontPaths.Length; i++)
        {
            try
            {
                string path = Path.Combine(RootDirectory, fontPaths[i]);
                Stream stream = TitleContainer.OpenStream(path);
                fontSystem.AddFont(stream);
            } catch
            {
                throw; //if the font file doesn't exist, this will explode. Which is good.
            }
        }
        loadedFonts.Add(fontName, fontSystem);
        return fontSystem;
    }

    public static FontSystem GetFontSystem(string fontName)
    {
        if (loadedFonts == null)
            throw new NullReferenceException("Trying to access fonts before assets have been loaded. Don't do that!");
        if (loadedFonts.TryGetValue(fontName, out FontSystem value))
            return value;
        else
            throw new NullReferenceException($"FontSystem with name '{fontName}' does not exist. Did you misspell it, or forget to create it?");
    }

    public static Texture2D LoadTexture(string name)
    {
        Texture2D texture = null;
        if (loadedTextures.TryGetValue(name, out WeakReference<Texture2D> value) && value.TryGetTarget(out texture))
        {
            return texture;
        }

        string path = Path.Combine(RootDirectory, TexturePath, name);
        foreach (string extension in supportedTexture2DExtensions)
        {
            try
            {
                using (Stream stream = TitleContainer.OpenStream(Path.ChangeExtension(path, extension)))
                {
                    if (stream != null)
                    {
                        texture = Texture2D.FromStream(RubedoEngine.Graphics.GraphicsDevice, stream, DefaultColorProcessors.PremultiplyAlpha);
                        if (!loadedTextures.ContainsKey(name))
                            loadedTextures.Add(name, new WeakReference<Texture2D>(texture));
                        else
                            loadedTextures[name] = new WeakReference<Texture2D>(texture);
                        return texture;
                    }
                }
            }
            catch { } //TODO: Make a better way to handle this whole thing.
        }
        throw new ContentLoadException($"Texture at path '{path}' does not exist!");
    }
}