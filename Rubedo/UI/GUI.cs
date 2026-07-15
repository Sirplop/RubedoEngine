using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Graphics.Sprites;
using System.Collections.Generic;

namespace Rubedo.UI;

/// <summary>
/// Base helper class for controlling the GUI.
/// </summary>
public static class GUI
{
    /// <summary>
    /// Set to false to disable UI input processing.
    /// </summary>
    public static bool DoUIInput { get; set; } = true;

    /// <summary>
    /// Controls when mouse vs keyboard/gamepad controls are used.
    /// </summary>
    public static bool MouseControlsEnabled { get; set; } = true;

    /// <summary>
    /// SpriteBatch used to draw the UI. We don't use a <see cref="Rubedo.Graphics.Renderer"/> because we're rendering screen coordinates.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; set; } = null!;

    /// <summary>
    /// Defaults to LinearClamp.
    /// </summary>
    public static SamplerState GuiSampler { get; set; } = SamplerState.PointClamp;

    /// <summary>
    /// Current GUIRoot.
    /// </summary>
    public static GUIRoot Root { get; set; } = null!;

    /// <summary>
    /// If true, draws colorful rectangles around each sublevel of UI.
    /// </summary>
    public static bool DebugDraw { get; set; } = false;
    public static int DebugDrawDepthMin { get; set; } = 1;

    private static readonly RasterizerState _rasterState = new RasterizerState { ScissorTestEnable = true };
    private static bool _beginCalled = false;
    private static readonly Stack<(Rectangle, bool)> _scissorStack = new Stack<(Rectangle, bool)>();

    internal static Matrix offsetMatrix;

    private static Texture2D testWhite;
    private static int _colorDepth = 0;
    private static Color _assemblyColor = new Color(1, 1, 1, 0.05f);
    private static Color[] _colorDepthColors = new Color[]
        {
            Color.DarkGray * _assemblyColor,
            Color.Blue * _assemblyColor,
            Color.Red * _assemblyColor,
            Color.Yellow * _assemblyColor,
            Color.Orange * _assemblyColor,
            Color.Purple * _assemblyColor,
            Color.Teal * _assemblyColor,
            Color.Beige * _assemblyColor,
            Color.Lime * _assemblyColor,
            Color.Magenta * _assemblyColor,
            Color.Cyan * _assemblyColor,
            Color.GhostWhite * _assemblyColor,
        };

    public static void Setup(Game game)
    {
        SpriteBatch = new SpriteBatch(game.GraphicsDevice);
        testWhite = new Texture2D(game.GraphicsDevice, 1, 1);
        testWhite.SetData(new[] { Color.White });
    }

    /* //immediate mode scissoring.
    public static void PushScissor(Rectangle r)
    {
        if (!_beginCalled)
        {
            Begin();
            _scissorStack.Push((SpriteBatch.GraphicsDevice.ScissorRectangle, true));
            SpriteBatch.GraphicsDevice.ScissorRectangle = r;
        }
        else
        {
            _scissorStack.Push((SpriteBatch.GraphicsDevice.ScissorRectangle, false));
            SpriteBatch.GraphicsDevice.ScissorRectangle = r;
        }
    }
    public static void PopScissor()
    {
        (SpriteBatch.GraphicsDevice.ScissorRectangle, bool wasBeginCalled) = _scissorStack.Pop();
        if (wasBeginCalled)
            End();
    }*/

    //deferred mode scissoring.
    public static void PushScissor(Rectangle r)
    {
        // TODO: Optimize begin call somehow. Maybe there is no drawing between scissor swaps?
        bool wasBeginCalled = _beginCalled;
        if (wasBeginCalled)
        {
            End();
        }
        Viewport view = SpriteBatch.GraphicsDevice.Viewport;
        int x = (int)(r.X * Root.Scale) + view.X; //must offset the scissor by the viewport.
        int y = (int)(r.Y * Root.Scale) + view.Y;
        int w = (int)(r.Width * Root.Scale);
        int h = (int)(r.Height * Root.Scale);


        _scissorStack.Push((SpriteBatch.GraphicsDevice.ScissorRectangle, wasBeginCalled));
        SpriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(x, y, w, h);
        Begin();
        if (DebugDraw)
        {
            int depth = _colorDepth++;
            if (depth > DebugDrawDepthMin)
                SpriteBatch.Draw(testWhite, SpriteBatch.GraphicsDevice.ScissorRectangle, _colorDepthColors[depth]);
        }
    }
    /// <summary>
    /// Uses a rectangle to limit the area that the spritebatch is allowed to draw to.
    /// </summary>
    public static void PopScissor()
    {
        if (DebugDraw)
        {
            _colorDepth--;
        }
        if (_beginCalled)
            End();

        bool wasBeginCalled;

        (SpriteBatch.GraphicsDevice.ScissorRectangle, wasBeginCalled) = _scissorStack.Pop();
        if (wasBeginCalled)
            Begin();
    }
    /// <summary>
    /// Calls begin on the spritebatch with the UI rasterizer state, transform matrix and sampler state.
    /// </summary>
    private static void Begin()
    {
        SpriteBatch.Begin(sortMode: SpriteSortMode.Deferred, rasterizerState: _rasterState, samplerState: GuiSampler, transformMatrix: Root.UIMatrix);
        _beginCalled = true;
    }
    /// <summary>
    /// Calls end on the spritebatch.
    /// </summary>
    private static void End()
    {
        SpriteBatch.End();
        _beginCalled = false;
    }
}