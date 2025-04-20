

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;

namespace Rubedo;

/// <summary>
/// I am AssetManager, and this is my summary.
/// </summary>
public static class AssetManager
{
    private static ContentManager _content;


    public static void Initialize(ContentManager contentManager)
    {
        _content = contentManager;
        //TODO: add missing asset things
    }
    public static SpriteFont LoadFont(string name)
    {
        return _content.Load<SpriteFont>("fonts/" + name);
    }
    public static Texture2D LoadTexture(string name)
    {
        try
        {
            return _content.Load<Texture2D>("textures/" + name);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static SoundEffect LoadSoundFx(string name)
    {
        try
        {
            return _content.Load<SoundEffect>("sound/" + name);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static Song LoadMusic(string name)
    {
        try
        {
            return _content.Load<Song>("music/" + name);
        }
        catch (Exception)
        {
            return null;
        }
    }
}