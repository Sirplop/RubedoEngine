//Heavily based off of Apos.Camera: https://github.com/Apostolique/Apos.Camera

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Rubedo.Rendering.Viewports;

/// <summary>
/// A full-window <see cref="SplitViewport"/>.
/// </summary>
public class DefaultViewport : SplitViewport
{
    public DefaultViewport(GraphicsDevice graphicsDevice, GameWindow window) : base(graphicsDevice, window, 0f, 0f, 1f, 1f) { }
}