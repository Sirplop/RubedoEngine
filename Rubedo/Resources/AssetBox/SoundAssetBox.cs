using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using SoLoud;
using System.IO;
using System;

namespace Rubedo.Resources.AssetBox;

/// <summary>
/// TODO: I am SoundAssetType, and I don't have a summary yet.
/// </summary>
public class SoundAssetBox : WeakAssetBox<Wav>
{
    internal static readonly string[] supportedAudioExtensions = new string[4] { ".wav", ".ogg", ".mp3", ".flac" };

    public SoundAssetBox(string name, string rootPath) : base(name, rootPath) { }

    public override void LoadAsset(string key, string[] extra = null)
    {
        if (key == string.Empty)
            return;

        if (AssetExists(key))
            return;

        Wav effect = new Wav();
        string path = Path.Combine(Assets.Location, Assets.RootDirectory, RootPath, key);
        foreach (string extension in supportedAudioExtensions)
        {
            try
            {
                string extPath = Path.ChangeExtension(path, extension);
                int returnCode = effect.load(extPath);
                if (returnCode != ReturnCode.SO_NO_ERROR)
                    continue;
                AddAsset(key, effect);
            }
            catch { }
        }

        throw new ContentLoadException($"Sound at '{path}' does not exist!");
    }

    public override void UnloadAsset(string key)
    {
        RemoveAsset(key);
    }
}