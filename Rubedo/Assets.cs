using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Graphics.Animation;
using Rubedo.Graphics.Sprites;
using Rubedo.Serializers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rubedo;

/// <summary>
/// Handles loading and unloading various assets.
/// </summary>
public static class Assets
{
    /// <summary>
    /// The directory path appended to the root directory for textures.
    /// </summary>
    public static string TexturePath = "textures";

    /// <summary>
    /// The directory path appended to the root directory for textures.
    /// </summary>
    public static string SoundsPath = "sounds";

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
    private static readonly string[] supportedAudioExtensions = new string[2] { ".wav", ".ogg" };

    private static Dictionary<string, FontSystem> loadedFonts; //fonts don't use a weak reference, because we want this to be persistent.
    private static Dictionary<string, WeakReference<Texture2D>> loadedTextures;
    private static Dictionary<string, WeakReference<SoundEffect>> loadedSounds;
    private static Dictionary<string, WeakReference<TextureAtlas2D>> loadedAtlases;
    private static Dictionary<string, WeakReference<IAnimation>> loadedAnimations;

    public static void Initialize(string rootDirectory)
    {
        RootDirectory = rootDirectory;
        loadedFonts = new Dictionary<string, FontSystem>(StringComparer.OrdinalIgnoreCase);
        loadedTextures = new Dictionary<string, WeakReference<Texture2D>>(StringComparer.OrdinalIgnoreCase);
        loadedAtlases = new Dictionary<string, WeakReference<TextureAtlas2D>>(StringComparer.OrdinalIgnoreCase);
        loadedSounds = new Dictionary<string, WeakReference<SoundEffect>>(StringComparer.OrdinalIgnoreCase);
        loadedAnimations = new Dictionary<string, WeakReference<IAnimation>>(StringComparer.OrdinalIgnoreCase);
    }
    #region Fonts
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
    #endregion
    #region Textures
    public static Texture2D LoadTexture(string name)
    {
        if (name == string.Empty)
            return null; //real quick n' dirty null propogation.

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
                            loadedTextures[name].SetTarget(texture);
                        return texture;
                    }
                }
            }
            catch { } //TODO: Make a better way to handle this whole thing.
        }
        throw new ContentLoadException($"Texture at '{path}' does not exist!");
    }

    public static TextureAtlas2D LoadAtlas(string atlasPath)
    {
        TextureAtlas2D atlas;
        if (loadedAtlases.TryGetValue(atlasPath, out WeakReference<TextureAtlas2D> value) && value.TryGetTarget(out atlas))
        {
            return atlas;
        }

        atlas = TextureAtlas2DLoader.Load(atlasPath);
        if (loadedAtlases.ContainsKey(atlasPath))
            loadedAtlases[atlasPath].SetTarget(atlas);
        else
            loadedAtlases.Add(atlasPath, new WeakReference<TextureAtlas2D>(atlas));

        return atlas;
    }
    #endregion
    #region Sounds
    public static SoundEffect LoadSoundEffect(string name)
    {
        SoundEffect effect = null;
        if (loadedSounds.TryGetValue(name, out WeakReference<SoundEffect> value) && value.TryGetTarget(out effect))
        {
            return effect;
        }

        string path = Path.Combine(RootDirectory, SoundsPath, name);
        foreach (string extension in supportedAudioExtensions)
        {
            try
            {
                using (Stream stream = TitleContainer.OpenStream(Path.ChangeExtension(path, extension)))
                {
                    if (stream != null)
                    {
                        effect = SoundEffect.FromStream(stream);
                        if (!loadedSounds.ContainsKey(name))
                            loadedSounds.Add(name, new WeakReference<SoundEffect>(effect));
                        else
                            loadedSounds[name].SetTarget(effect);
                        return effect;
                    }
                }
            }
            catch { }
        }

        throw new ContentLoadException($"Sound at '{path}' does not exist!");
    }
    #endregion
    #region Animations
    public static T LoadAnimation<T>(string name) where T : IAnimation
    {
        return LoadAnimation<T>(name, TexturePath);
    }
    public static T LoadAnimation<T>(string name, string root) where T : IAnimation
    {
        if (loadedAnimations.TryGetValue(name, out WeakReference<IAnimation> animRef) && animRef.TryGetTarget(out IAnimation anim))
        {
            if (anim.GetType() == typeof(T))
                return (T)anim;
            throw new ContentLoadException($"Animation with name '{name}' present in loaded assets, but it's not the right type!");
        }
        string path = Path.Combine(RootDirectory, root, name);
        if (typeof(T) == typeof(SpriteAnimation))
        { //TODO: Do this better - this'll get really ugly the more animation types we have.
            anim = LoadSpriteAnimation(path);
            if (loadedAnimations.ContainsKey(name))
                loadedAnimations[name].SetTarget(anim);
            else
                loadedAnimations.Add(name, new WeakReference<IAnimation>(anim));
            return (T)anim;
        }
        throw new ContentLoadException($"Animation at '{path}' does not exist, or is not of the appropriate type!");
    }

    private static SpriteAnimation LoadSpriteAnimation(string path)
    {
        return SpriteAnimLoader.Load(path);
    }
    #endregion
}