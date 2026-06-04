using Microsoft.Xna.Framework.Content;
using Rubedo.Graphics.Animation;
using System.IO;
using System;
using Rubedo.Resources.Serializers;

namespace Rubedo.Resources.AssetBox;

/// <summary>
/// TODO: I am AnimationAssetType, and I don't have a summary yet.
/// </summary>
public class AnimationAssetBox<T> : WeakAssetBox<T> where T : class, IAnimation
{
    public AnimationAssetBox(string name, string rootPath) : base(name, rootPath) { }

    public override void LoadAsset(string key, string[] extra = null)
    {
        if (key == string.Empty)
            return;

        if (AssetExists(key))
        {
            IAnimation anima = GetAsset(key);
            if (anima.GetType() == typeof(T))
                return;
            throw new ContentLoadException($"Animation with name '{key}' present in loaded assets, but it's not the right type!");
        }

        string path = Path.Combine(Assets.RootDirectory, RootPath, key);
        if (typeof(T) == typeof(SpriteAnimation))
        { //TODO: Do this better - this'll get really ugly the more animation types we have.
            IAnimation anim = LoadSpriteAnimation(path);
            AddAsset(key, (T)anim);
            return;
        }
        throw new ContentLoadException($"Animation at '{path}' does not exist, or is not of the appropriate type!");
    }

    //Temporary until I have a reason to break this up for new animation types.
    private static SpriteAnimation LoadSpriteAnimation(string path)
    {
        return SpriteAnimLoader.Load(path);
    }

    public override void UnloadAsset(string key)
    {
        throw new NotImplementedException();
    }
}