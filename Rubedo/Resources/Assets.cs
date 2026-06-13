using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Audio;
using Rubedo.Graphics.Animation;
using Rubedo.Graphics.Sprites;
using Rubedo.Resources.AssetBox;
using Rubedo.Resources.Serializers;
using SoLoud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Rubedo.Resources;

/// <summary>
/// Handles loading and unloading various assets.
/// </summary>
public static class Assets
{
    #region Monogame TitleContainer & CurrentPlatform
    private enum OS
    {
        Windows,
        Linux,
        MacOSX,
        Unknown
    }

    private static OS _os;

    ///need to copy out parts of <see cref="TitleContainer"/> and CurrentPlatform  because they're internal. Grumble grumble.
    internal static string Location { get; private set; }

    [DllImport("libc")]
    private static extern int uname(nint buf);
    private static void PlatformInit()
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.Win32NT:
            case PlatformID.WinCE:
                _os = OS.Windows;
                break;
            case PlatformID.MacOSX:
                _os = OS.MacOSX;
                break;
            case PlatformID.Unix:
                {
                    _os = OS.MacOSX;
                    nint num = nint.Zero;
                    try
                    {
                        num = Marshal.AllocHGlobal(8192);
                        if (uname(num) == 0 && Marshal.PtrToStringAnsi(num) == "Linux")
                        {
                            _os = OS.Linux;
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (num != nint.Zero)
                        {
                            Marshal.FreeHGlobal(num);
                        }
                    }

                    break;
                }
            default:
                _os = OS.Unknown;
                break;
        }

        if (_os == OS.MacOSX)
        {
            Location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources");
            if (!Directory.Exists(Location))
            {
                Location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Resources");
            }
        }

        if (!Directory.Exists(Location))
        {
            Location = AppDomain.CurrentDomain.BaseDirectory;
        }
    }

    static Assets()
    {
        Location = string.Empty;
        PlatformInit();
    }
    #endregion

    private static readonly Dictionary<string, IAssetBox> _assets = new Dictionary<string, IAssetBox>(StringComparer.OrdinalIgnoreCase);

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

    public static void RegisterAssetBox(string key, IAssetBox assetType)
    {
        _assets.Add(key, assetType);
    }
    public static void UnregisterAssetBox(string key)
    {
        _assets.Remove(key);
    }

    public static IAssetBox? GetAssetBox(string key)
    {
        if (_assets.TryGetValue(key, out IAssetBox assetType))
        {
            return assetType;
        }

        return null;
    }

    /// <summary>
    /// Gets a resource from the given asset type with the given key path. Asset type defaults to the type class name.
    /// </summary>
    public static T GetResource<T>(string resourceKey, string typeKey = "") where T : class
    {
        if (typeKey == string.Empty)
        {
            typeKey = typeof(T).Name.ToLower();
        }

        if (_assets.TryGetValue(typeKey, out IAssetBox assetType))
        {
            if (assetType.IsWeakReference)
            {
                return ((WeakAssetBox<T>)assetType).GetAsset(resourceKey);
            }
            else
            {
                return ((StrongAssetBox<T>)assetType).GetAsset(resourceKey);
            }
        }

        Log.Error($"'{typeKey}' at '{resourceKey}' does not exist!");

        return null;
    }

    public static bool TryGetResource<T>(string resourceKey, out T value, string typeKey = "") where T : class
    {
        if (typeKey == string.Empty)
        {
            typeKey = typeof(T).Name.ToLower();
        }

        if (_assets.TryGetValue(typeKey, out IAssetBox assetType))
        {
            if (assetType.IsWeakReference)
            {
                return ((WeakAssetBox<T>)assetType).TryGetAsset(resourceKey, out value);
            }
            else
            {
                return ((StrongAssetBox<T>)assetType).TryGetAsset(resourceKey, out value);
            }
        }

        value = default;
        return false;
    }

    public static void Initialize(string rootDirectory)
    {
        RootDirectory = rootDirectory;
        _ = new TextureAssetBox(string.Empty, "textures");
        _ = new TextureAtlasAssetBox(string.Empty, "textures");
        _ = new FontAssetBox(string.Empty, "fonts");
        _ = new SoundAssetBox(string.Empty, "sounds");
        _ = new AnimationAssetBox<SpriteAnimation>(string.Empty, "textures");
    }

    #region Fonts
    /// <summary>
    /// Create a new font system with the given lowercase <paramref name="fontName"/>, which includes the given <paramref name="fontPaths"/>.
    /// </summary>
    /// <returns>The new font system, or the existing one if it already exists.</returns>
    /// <remarks>The font paths are relative to the content root path.</remarks>
    public static FontSystem CreateNewFontSystem(string fontName, params string[] fontPaths)
    {
        IAssetBox box = GetAssetBox("fontsystem");
        box.LoadAsset(fontName, fontPaths);
        return GetFont(fontName);
    }

    public static FontSystem GetFont(string fontName)
    {
        return GetResource<FontSystem>(fontName);
    }
    #endregion
    #region Sound Stream
    /// <summary>
    /// Loads a stream for the given sound. Loads the root path from the default sound asset box.
    /// </summary>
    /// <remarks>This does not result in the caching of the stream.</remarks>
    public static WavStream LoadSoundStream(string name)
    {
        WavStream effect = new WavStream();
        string path = Path.Combine(Location, RootDirectory, GetAssetBox("wav").RootPath, name);
        foreach (string extension in SoundAssetBox.supportedAudioExtensions)
        {
            try
            {
                string extPath = Path.ChangeExtension(path, extension);
                int returnCode = effect.load(extPath);
                if (returnCode != ReturnCode.SO_NO_ERROR)
                    continue;
                return effect;
            }
            catch { }
        }

        throw new ContentLoadException($"Sound at '{path}' does not exist!");
    }
    #endregion
}