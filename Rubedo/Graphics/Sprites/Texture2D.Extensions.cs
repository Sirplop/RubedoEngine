
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Resources;

namespace Rubedo.Graphics.Sprites;


/// <summary>
/// Extension methods for <see cref="Texture2D"/>.
/// </summary>
public static class Texture2DExtensions
{
    /// <summary>
    /// Creates a 1x1 texture of the given color.
    /// </summary>
    public static Texture2D CreateSolidColor()
    {
        if (Assets.TryGetResource<Texture2D>("__1x1color", out Texture2D texture))
        {
            return texture;
        }

        texture = new Texture2D(RubedoEngine.Graphics.GraphicsDevice, 1, 1);
        texture.SetData<Color>(new Color[] { Color.White });

        IAssetBox? box = Assets.GetAssetBox("texture2d");
        if (box == null)
        {
            throw new Microsoft.Xna.Framework.Content.ContentLoadException("Missing texture2d asset box, cannot create __1x1color texture!");
        }
        ((WeakAssetBox<Texture2D>)box).AddAsset("__1x1color", texture);

        return texture;
    }
}